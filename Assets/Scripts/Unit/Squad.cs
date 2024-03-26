using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Capstone 
{
    public class Squad : Unit
    {
        public SquadData squadData;
        public GameObject[] squadMembers;
        private float captureRate;
        private int veterancy;
        private GameObject squadLead; // Reference to the 'primary' member of the squad - determines where the unit icon is rendered, upon death another member
                                       // is assigned to this - will modify code to assume squadLead is always = 0th unit
        private int squadSize;
        private int aliveMembers; // Tracks current size of the squad to allow for reinforcement and generate spacings 
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
            Dictionary<GameObject, RaycastHit> movePositions = getOptimalMovePositions(hits);
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
        /// 
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        private Dictionary<GameObject, RaycastHit> getOptimalMovePositions(List<RaycastHit> hits) 
        {
            List<Dictionary<GameObject, RaycastHit>> permutations = getPermutations(squadMembers, hits);

            Dictionary<GameObject, RaycastHit> returnDict = new Dictionary<GameObject, RaycastHit>();
            
            float minH = 99999f;

            foreach (Dictionary<GameObject, RaycastHit> permutation in permutations)
            {   
                float cost = 0f;
                foreach (var pair in permutation)
                {
                    cost += Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                }
                if (cost < minH && permutation[squadLead].point == hits[0].point)
                {
                    minH = cost;
                    returnDict = permutation;
                }
            }
            return returnDict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="GameObject"></typeparam>
        /// <typeparam name="RaycastHit"></typeparam>
        /// <param name="members"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        private List<Dictionary<GameObject, RaycastHit>> getPermutations<GameObject, RaycastHit>(GameObject[] members, List<RaycastHit> hits)
        {
            List<Dictionary<GameObject, RaycastHit>> result = new List<Dictionary<GameObject, RaycastHit>>();
            int len = members.Length;
            RaycastHit[] hitsArray = hits.ToArray();

            RaycastHit squadLeadPosition = hits[0];

            void heaps(int l)
            {
                if (l == 1)
                {
                    Dictionary<GameObject, RaycastHit> temp = new Dictionary<GameObject, RaycastHit>();
                    for (int i=0; i < len; i++)
                    {
                        if (i == 0) {
                            temp[members[i]] = squadLeadPosition;
                        } else {
                            temp[members[i]] = hitsArray[i];
                        }
                    }
                    result.Add(temp);
                } 
                else 
                {
                    for (int i=0; i < len; i++)
                    {
                        heaps(l-1);

                        if (l % 2 == 1)
                        {
                            (members[i], members[l - 1]) = (members[l - 1], members[i]);
                        } else {
                            (members[0], members[l - 1]) = (members[l - 1], members[0]);
                        }
                    }
                }
            }
            heaps(len);
            return result;
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
            Dictionary<GameObject, RaycastHit> movePositions = getOptimalMovePositions(hits);
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
                Debug.Log("Icon Position Object not found!");
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

            // Updating the Squad UI Icon
            uiIcon.setAliveModels(aliveMembers);
            uiIcon.setHealtBarColor(Color.blue);
            uiIcon.setCurrentHealth(1.0f);
            uiIcon.setReferenceUnit(this);
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
            setSquadWorldUI(unitIconWorld);
        }

        // ---------------------- Getter / Setter methods ------------------------------
        public int getAliveMembers() {
            return this.aliveMembers;
        }

        public Transform getCurrentTransform() {
            return this.transform;
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
            Debug.Log("Dealing damage!");
            foreach (GameObject squadMember in squadMembers)
            {
                SquadMember squadMemberComponent = squadMember.GetComponent<SquadMember>();
                squadMemberComponent.takeDamage(damage);
            }
        }

    }
}

