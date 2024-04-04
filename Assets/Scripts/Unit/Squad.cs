using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Capstone 
{
    public class Squad : Unit
    {
        // Private, Editor Accessible Variables
        [SerializeField] private GameObject[] models;
        [SerializeField] private float captureRate;

        // Private, Runtime Variables
        [SerializeField] private List<GameObject> squadMembers = new List<GameObject>();
        private int veterancy;
        [SerializeField] private GameObject squadLead; // Reference to the 'primary' member of the squad - determines where the unit iconSprite isiconSpritesered, upon death another member
                                       // is assigned to this - will modify code to assume squadLead is always = 0th unit
        private int squadSize;
        private int aliveMembers; // Tracks current size of the squad to allow for reinforcement and generate spacings 
        public SquadState squadState = SquadState.stationary;
        
        // --------------------- Class Methods ---------------------------------

        public void Awake()
        {

        }
        public override void Start()
        {
            base.Start();
            StartCoroutine(InstantiateSquad());
        }

        void Update()
        {
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
                if (member != null)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {   
                        render.enabled = true;
                    }
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
                if (member != null)
                {
                    SpriteRenderer render = member.GetComponentInChildren<SpriteRenderer>();
                    if (render != null)
                    {   
                        render.enabled = false;
                    }
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
            if (modelObject == squadLead)
            {
                squadLead = null;
            }
            squadMembers.Remove(modelObject);
            Destroy(modelObject);
            aliveMembers--;
            SquadMoveMarkerPool.sharedInstance.updateVisibleMarkers(aliveMembers);
            worldIconComponent.setAliveModels(aliveMembers);
            if (uiIconComponent != null)
                uiIconComponent.setAliveModels(aliveMembers);
            if (aliveMembers >= 1)
            {

                if (squadLead == null)
                {
                    squadLead = squadMembers[0];
                    GameObject iconPosition = squadLead.transform.Find("iconPos").gameObject;
                    if (iconPosition != null) 
                        iconPosition.SetActive(true);
                    worldIconComponent.setReferencePosition(iconPosition);
                }
                updateUIHealth();
            }
            else {
                killSquad();
            }
        }
        private void killSquad()
        {
            if (aliveMembers <= 0) {
                if (owner is Player)
                {
                    GameManager.Instance.playerUI.removeUnitIcon(uiIconComponent.gameObject);
                    GameManager.Instance.playerUI.removeWorldSpaceUnitIcon(worldIconComponent);
                    base.deselect();
                } else {
                    GameManager.Instance.playerUI.removeWorldSpaceUnitIcon(worldIconComponent);
                }
                Destroy(this);
            }
        }

        // Fog of War Code

        private IEnumerator fogLoop()
        {
            bool previousRevealed = true;

            while (true)
            {
                bool currentRevealed = checkReveal();
                
                if (currentRevealed != previousRevealed)
                {
                    revealStatus = currentRevealed;
                    setSquadVisibility(currentRevealed);
                    previousRevealed = currentRevealed;
                }

                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        public override bool checkReveal()
        {
            if (FogLayerManager.instance.getPlayerTeam() != team)
            {
                foreach (GameObject squadMember in squadMembers)
                {
                    if (squadMember != null)
                    {
                        if (FogLayerManager.instance.isInFog(squadMember.transform.position))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return true;
        }

        public void setSquadVisibility(bool state)
        {
            if (state)
            {
                int selectableLayer = LayerMask.NameToLayer("Visible");
                foreach (GameObject member in squadMembers)
                {
                    if (member != null)
                        member.layer = selectableLayer;
                }       
            } else {
                int hiddenLayer = LayerMask.NameToLayer("Hidden");
                foreach (GameObject member in squadMembers)
                {
                    if (member != null)
                        member.layer = hiddenLayer;
                }
            }
        }

        // ---- Initialization Code ----

        /// <summary>
        /// Used when spawning a new squad. Generates a new squad at target coordinates with relevant variables
        /// </summary>
        private IEnumerator InstantiateSquad() 
        {
            while (models == null)
            {
                yield return null;
            }
            // Initializing the SquadList, the intended SquadSize, and 
            //"aliveMembers" which is used to track the current alive members of the squad
            squadSize = models.Length;

            aliveMembers = squadSize;
      
            List<Vector3> spacing = unitFunctions.generateSpacing(transform.position, aliveMembers);

            // Setting the squad's iconSprite baiconSpritesn whether or not it belongs to player's team
            if (owner.team != GameManager.Instance.player.team) 
            {
                iconSprite = iconSprites[1];
            } else {
                iconSprite = iconSprites[0];
            }

            // Spawn loop for initializing each model of squad according to 
            // the associated model prefabs
            for (int i = 0; i < squadSize; i++)
            {   
                // Spawns the prefab and ties the reference to the squadMembers list
                squadMembers.Add(Instantiate(models[i], spacing[i], Quaternion.identity));

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

            // Initializes the squad Icon and attaches it to the iconSprite poiconSpritesn empty object on the model.
            GameObject iconPosition = squadLead.transform.Find("iconPos").gameObject;
            if (iconPosition != null) {
                iconPosition.SetActive(true);

                worldIconComponent = GameManager.Instance.playerUI.spawnWorldSpaceUnitIcon(iconObj, iconPosition);
                worldIconObj = worldIconComponent.gameObject;

                StartCoroutine(initializeSquadIcon(worldIconObj.GetComponent<UnitIconUIWorld>()));
            } else {
                //Debug.Log("Icon Position Object not found!");
            }

            // Setting the render color of the selection circles
            if (owner.team != GameManager.Instance.player.team)
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
            if (owner.team != GameManager.Instance.player.team)
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
            if (owner == GameManager.Instance.player)
            {
                if (uiIconComponent != null)
                {
                    uiIconComponent.setAliveModels(aliveMembers);
                    uiIconComponent.setHealtBarColor(0);
                    uiIconComponent.setCurrentHealth(1.0f);
                    uiIconComponent.setReferenceUnit(this);
                } else {
                    // Accounting for Debug Spawning
                    uiIconComponent = GameManager.Instance.playerUI.addNewUnitIcon(getPortrait(), getIcon(0));
                    setSquadIconUI(uiIconComponent);
                    uiIconComponent.setAliveModels(aliveMembers);
                    uiIconComponent.setHealtBarColor(0);
                    uiIconComponent.setCurrentHealth(1.0f);
                    uiIconComponent.setReferenceUnit(this);
                }
            }
            
            StartCoroutine(fogLoop());
            yield break;
        }

        private IEnumerator initializeSquadIcon(UnitIconUIWorld unitIconUIWorld)
        {
            while (unitIconUIWorld.checkFullyInitialized() == false)
            {
                yield return null;
            }
            
            worldIconComponent = unitIconUIWorld;
            unitIconUIWorld.setIconTag("SquadIcon");
            unitIconUIWorld.setUnitIcon(iconSprite);   
            unitIconUIWorld.setAliveModels(aliveMembers);
            unitIconUIWorld.setUnitReference(this.gameObject);
            unitIconUIWorld.setReferenceUnitComponent(this);

            Player playerObj = FindObjectOfType<Player>();
            if (owner.team != playerObj.team)
                unitIconUIWorld.setHealthBarColor(UnitIconRender.enemy);
            else if (owner.team == playerObj.team && owner.ownerTag != playerObj.ownerTag)
                unitIconUIWorld.setHealthBarColor(UnitIconRender.playerTeam);
            else    
                unitIconUIWorld.setHealthBarColor(UnitIconRender.player);

            while (getCurrentSquadHealth() == 0f)
            {
                yield return null;
            }
            unitIconUIWorld.setCurrentHealth(getCurrentSquadHealth());
            setSquadWorldUI(unitIconUIWorld);
        }

        // ---------------------- Getter / Setter methods ------------------------------
        public int getAliveMembers() { return this.aliveMembers; }

        public Transform getCurrentTransform() {
            if (this.squadLead != null)
                return this.squadLead.transform;
            return null;
        }

        private float getSquadModelSpeed() {
            SquadMember component = squadMembers[0].GetComponent<SquadMember>();
            return component.getMoveSpeed();
        }

        public float getSquadCaptureRate() { return this.captureRate; }

        private float getCurrentSquadHealth()
        {
            float currentHealth = 0.0f;
            foreach(GameObject model in squadMembers)
            {
                if (model != null)
                {
                    SquadMember squadMember = model.GetComponent<SquadMember>();
                    currentHealth += squadMember.getCurrentHealth();
                }
            }
            return currentHealth / getSquadMaxHealth() * 1.0f;
        }

        private float getSquadMaxHealth()
        {
            float maxHealth = 0.0f;
            foreach(GameObject model in models)
            {
                SquadMember squadMember = model.GetComponent<SquadMember>();
                maxHealth += squadMember.getMaxHealth();
            }
            return maxHealth;
        }

        public void setSquadIconUI(UnitIconUI unitIconUI) { uiIconComponent = unitIconUI; }

        public void setSquadWorldUI(UnitIconUIWorld unitIconWorld) { worldIconComponent = unitIconWorld; }

        public void updateUIHealth()
        {
            if (uiIconComponent != null)
            {
                uiIconComponent.setCurrentHealth(getCurrentSquadHealth());
                worldIconComponent.setCurrentHealth(getCurrentSquadHealth());
            } else {
                worldIconComponent.setCurrentHealth(getCurrentSquadHealth());
            }
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

