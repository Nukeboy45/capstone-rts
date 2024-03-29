using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Capstone 
{
    public class Squad : Unit
    {
        // Public Variables
        [SerializeField] public SquadData squadData;

        // Private, Runtime Variables
        private GameObject[] squadMembers;
        private float captureRate;
        private int veterancy;
        private GameObject squadLead; // Reference to the 'primary' member of the squad - determines where the unit icon is rendered, upon death another member
                                       // is assigned to this - will modify code to assume squadLead is always = 0th unit
        private int squadSize;
        private int aliveMembers; // Tracks current size of the squad to allow for reinforcement and generate spacings 
        private bool revealed = true;
        public SquadState squadState = SquadState.stationary;
        
        // --------------------- Class Methods ---------------------------------

        public override void Start()
        {
            base.Start();
            InstantiateSquad();
        }

        void Update()
        {
            if (aliveMembers <= 0) {
                if (owner is Player)
                {
                    Player playerComponent = (Player)owner;
                    playerComponent.playerUI.removeUnitIcon(uiIcon.gameObject);
                    Destroy(uiIcon.gameObject);
                }
                base.deselect();
                Destroy(this.gameObject);
            }

            if (showSelect != null && selected == false)
            {
                if (showSelect == true)
                {
                    showSelectionRadius();
                    showSelect = false;
                } else {
                    hideSelectionRadius();
                    showSelect = false;
                }
            }
        }

        /// <summary>
        /// Calls Unit select method and calls to show selection radiuses for this squad
        /// </summary>
        public override void select()
        {
            base.select();
            selected = true;
            showSelectionRadius();
        }

        /// <summary>
        /// Calls Unit deselect method and calls to hide selection radiuses for this squad
        /// </summary>
        public override void deselect()
        {
            base.deselect();
            selected = false;
            hideSelectionRadius();
        }

        /// <summary>
        /// Overrides the parent moveTo command and issues move commands to each squadMember
        /// </summary>
        /// <param name="hit"></param>
        public override void moveTo(List<RaycastHit> hits)
        {
            Dictionary<GameObject, RaycastHit> movePositions = unitFunctions.getOptimalMovePositions(hits, squadMembers, squadLead);
            float longestMoveTime = getMaxMoveTime(movePositions, getSquadModelSpeed());
            foreach (var pair in movePositions)
            {
                SquadMember component = pair.Key.GetComponent<SquadMember>();
                NavMeshAgent agent = pair.Key.GetComponent<NavMeshAgent>();
                float distance = Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                float speed = getNormalizedMoveSpeed(distance, longestMoveTime, getSquadModelSpeed());
                component.moveToPosition(pair.Value.point, speed);
            }
        }

        private float getMaxMoveTime(Dictionary<GameObject, RaycastHit> movePairs , float speed)
        {   
            float maxTime = 0f;
            foreach (var pair in movePairs)
            {
                float tempTime = Vector3.Distance(pair.Key.transform.position, pair.Value.point) / speed;
                if (tempTime > maxTime)
                {
                    maxTime = tempTime;
                }
            }
            return maxTime;
        }

        private float getNormalizedMoveSpeed(float distance, float maxMoveTime, float moveSpeed)
        {
            return distance / maxMoveTime;
        }

        /// <summary>
        /// Shows the selection radiuses for squad members
        /// </summary>
        private void showSelectionRadius()
        {
            foreach (GameObject member in squadMembers)
            {
                SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                if (render != null)
                {   
                    render.enabled = true;
                }
            }
        }

        /// <summary>
        /// Hides the selection radiuses for squad members
        /// </summary>
        private void hideSelectionRadius()
        {
            foreach (GameObject member in squadMembers)
            {
                SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                if (render != null)
                {   
                    render.enabled = false;
                }
            }
        }

        public void retreat(List<RaycastHit> hits)
        {
            squadState = SquadState.retreating;
            Dictionary<GameObject, RaycastHit> movePositions = unitFunctions.getOptimalMovePositions(hits, squadMembers, squadLead);
            float longestMoveTime = getMaxMoveTime(movePositions, 10.0f);
            foreach (var pair in movePositions)
            {
                SquadMember component = pair.Key.GetComponent<SquadMember>();
                NavMeshAgent agent = pair.Key.GetComponent<NavMeshAgent>();
                float distance = Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                float speed = getNormalizedMoveSpeed(distance, longestMoveTime, getSquadModelSpeed());
                component.moveToPosition(pair.Value.point, speed);
            }
        }

        /// <summary>
        /// Function called by a squadMember object belonging to this squad when its
        /// health reaches 0. Allows the squad to always determine proper number of 
        /// alive members.
        /// </summary>
        /// <param name="modelObject"></param>
        public void killModel(GameObject modelObject) 
        {
            Destroy(modelObject);
            aliveMembers--; 
        }

        // Fog of War Code

        private IEnumerator fogLoop()
        {
            while (true)
            {
                while (checkRevealed() == false)
                {
                    if (revealed != false)
                    {
                        revealed = false;
                        setSquadVisibility(false);
                    }
                    yield return null;
                }

                if (revealed != true)
                {
                    revealed = true;                    
                    setSquadVisibility(true);
                }
                yield return new WaitForSecondsRealtime(1);
            }
        }
        public override bool checkRevealed()
        {
            if (FogLayerManager.instance.getPlayerTeam() != this.team)
            {
                foreach (GameObject squadMember in squadMembers)
                {
                    if (squadMember != null)
                    {
                        if (FogLayerManager.instance.isInFog(squadMember.transform.position) && revealed == false)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            // Default for Player / Ally Units
            return true;
        }

        public void setSquadVisibility(bool state)
        {
            Debug.Log("Calling");
            if (state)
            {
                int selectableLayer = LayerMask.NameToLayer("Selectable");
                foreach (GameObject member in squadMembers)
                {
                    member.layer = selectableLayer;
                }       
            } else {
                int hiddenLayer = LayerMask.NameToLayer("Hidden");
                foreach (GameObject member in squadMembers)
                {
                    member.layer = hiddenLayer;
                }
            }
        }

        // ---- Initialization Code ----

        /// <summary>
        /// Used when spawning a new squad. Generates a new squad at target coordinates with relevant variables
        /// </summary>
        void InstantiateSquad() 
        {
            // Initializing the SquadList, the intended SquadSize, and 
            //"aliveMembers" which is used to track the current alive members of the squad
            squadSize = squadData.models.Length;

            aliveMembers = squadSize;

            squadMembers = new GameObject[squadSize];

            captureRate = squadData.captureRate;
            
            List<Vector3> spacing = unitFunctions.generateSpacing(transform.position, aliveMembers);

            // Setting the squad's icon based on whether or not it belongs to player's team
            Player playerObj = FindObjectOfType<Player>();
            if (owner.team != playerObj.team) 
            {
                squadData.icon = squadData.icons[1];
            } else {
                squadData.icon = squadData.icons[0];
            }

            // Spawn loop for initializing each model of squad according to 
            // the associated model prefabs
            for (int i = 0; i < squadSize; i++)
            {   
                // Spawns the prefab and ties the reference to the squadMembers list
                squadMembers[i] = Instantiate(squadData.models[i], spacing[i], Quaternion.identity);

                // Retrieves the squadMember script reference from the newly instantiated
                // infantry model
                SquadMember squadMemberComponent = squadMembers[i].GetComponent<SquadMember>();

                // Sets the parent reference on the model to the current squad object
                squadMemberComponent.parent = this;

                // First model to spawn becomes the default "squadLead" - 
                // squadLead models do not get any bonuses, their coordinates are used to 
                // render the squadIcon. Upon the death of a squadLead, 
                // the reference will automatically transfer to another squadMember object
                if (i == 0 & squadLead == null) {
                    squadLead = squadMembers[i];
                }

                // Sets the squad's transformation as parent - not really relevant since
                // movement code is always separate, but makes it look cleaner in the
                // editor
                //squadMembers[i].transform.SetParent(this.transform, false);
            }

            // Initializes the squad Icon and attaches it to the icon position empty object on the model.
            GameObject iconPosition = squadLead.transform.Find("iconPos").gameObject;
            if (iconPosition != null) {
                iconPosition.SetActive(true);
                
                GameObject iconObject = Instantiate(squadData.iconObj, new Vector3(0,0,0), Quaternion.identity);
                iconObject.transform.SetParent(iconPosition.transform, false);

                UnitIconWorld unitIconWorld = iconObject.GetComponent<UnitIconWorld>();

                StartCoroutine(initializeSquadIcon(unitIconWorld));
                
                iconObject.layer = LayerMask.NameToLayer("Selectable");
            } else {
                //Debug.Log("Icon Position Object not found!");
            }

            // Setting the render color of the selection circles
            if (owner.team != playerObj.team)
            {
                foreach(GameObject member in squadMembers)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {
                    render.color = Color.red;
                    render.enabled = false;
                    }
                }
            } else {
                foreach(GameObject member in squadMembers)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {
                        render.color = Color.blue;
                        render.enabled = false;
                    }
                }    
            }

            // Deactivating / Activating FOV sight based on team
            if (owner.team != playerObj.team)
            {
                foreach(GameObject member in squadMembers)
                {
                    SquadMember squadMemberComponent = member.GetComponent<SquadMember>();
                    squadMemberComponent.setSightRadiusStatus(false);
                }
            } else {
                foreach(GameObject member in squadMembers)
                {
                    SquadMember squadMemberComponent = member.GetComponent<SquadMember>();
                    squadMemberComponent.setSightRadiusStatus(true);
                }     
            }

            // Updating the Squad UI Icon
            if (owner == playerObj)
            {
                uiIcon.setAliveModels(aliveMembers);
                uiIcon.setHealtBarColor(new Color(0f, 0.04f, 0.42f, 1.0f));
                uiIcon.setCurrentHealth(1.0f);
                uiIcon.setReferenceUnit(this);
            }

            StartCoroutine(fogLoop());
        }

        private IEnumerator initializeSquadIcon(UnitIconWorld unitIconWorld)
        {
            while (unitIconWorld.checkFullyInitialized() == false)
            {
                yield return null;
            }

            unitIconWorld.setCurrentHealth(getCurrentSquadHealth());
            unitIconWorld.setUnitIcon(squadData.icon);
            unitIconWorld.setAliveModels(aliveMembers);

            Player playerObj = FindObjectOfType<Player>();
            if (owner.team != playerObj.team)
                unitIconWorld.setHealthBarColor(UnitIconRender.enemy);
            else if (owner.team == playerObj.team && owner.ownerTag != playerObj.ownerTag)
                unitIconWorld.setHealthBarColor(UnitIconRender.playerTeam);
            else    
                unitIconWorld.setHealthBarColor(UnitIconRender.player);
            setSquadWorldUI(unitIconWorld);
        }

        // ---------------------- Getter / Setter methods ------------------------------
        public int getAliveMembers() {
            return this.aliveMembers;
        }

        public Transform getCurrentTransform() {
            if (this.squadLead != null)
                return this.squadLead.transform;
            return null;
        }

        public float getSquadCaptureRate()
        {
            return this.captureRate;
        }

        public void setSquadIconUI(UnitIconUI unitIconUI)
        {
            uiIcon = unitIconUI;
        }

        public void setSquadWorldUI(UnitIconWorld unitIconWorld)
        {
            worldIcon = unitIconWorld;
        }

        public void updateUIHealth()
        {
            uiIcon.setCurrentHealth(getCurrentSquadHealth());
            worldIcon.setCurrentHealth(getCurrentSquadHealth());
        }

        private float getSquadModelSpeed() {
            SquadMember component = squadMembers[0].GetComponent<SquadMember>();
            return component.moveSpeed;
        }

        private float getCurrentSquadHealth()
        {
            float currentHealth = 0.0f;
            foreach(GameObject model in squadMembers)
            {
                SquadMember squadMember = model.GetComponent<SquadMember>();
                currentHealth += squadMember.getCurrentHealth();
            }
            return currentHealth / getSquadMaxHealth() * 1.0f;
        }

        private float getSquadMaxHealth()
        {
            float maxHealth = 0.0f;
            foreach(GameObject model in squadData.models)
            {
                SquadMember squadMember = model.GetComponent<SquadMember>();
                maxHealth += squadMember.maxHealth;
            }
            return maxHealth;
        }

        // -------------- Debug Functions -------------------

        public void dealDebugDamage(float damage)
        {
            //Debug.Log("Dealing damage!");
            foreach (GameObject squadMember in squadMembers)
            {
                SquadMember squadMemberComponent = squadMember.GetComponent<SquadMember>();
                squadMemberComponent.takeDamage(damage);
            }
        }

    }
}

