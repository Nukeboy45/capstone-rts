using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone 
{
    public class Squad : Unit
    {
        // Private, Editor Accessible Variables
        [SerializeField] private GameObject[] models;
        [SerializeField] private float captureRate;

        // Private, Runtime Variables
        [SerializeField] private List<GameObject> squadMembers = new List<GameObject>();
        [SerializeField] private List<int> reinforceMemberIndexes = new List<int>();
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
            // StartCoroutine(squadUpdate());
        }

        void Update()
        {
            if (Time.timeScale != 0)
            {
                fogCheck();
                if (!multiSelect)
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
                } else {
                    if (Time.time - multiSelectTime > 0.1f)
                    {
                        multiSelect = false;
                        hideSelectionRadius();
                    } else {
                        showSelectionRadius();
                    }
                }

                if (squadState == SquadState.retreating)
                {
                    if (checkStopped(0.6f))
                        squadState = SquadState.stationary;
                }

                if (squadState == SquadState.moving || squadState == SquadState.attackmove)
                {
                    if (team == FogLayerManager.Instance.getPlayerTeam())
                        FogLayerManager.Instance.isDirty = true;
                    if (checkStopped(0.4f))
                        squadState = SquadState.stationary;
                }
            }
        }

        private IEnumerator squadUpdate()
        {
            while (true)
            {
                while (Time.timeScale == 0)
                {
                    yield return null;
                }
                fogCheck();
                if (!multiSelect)
                {
                    // if (Time.time - showSelectTime > 0.1f || showSelectTime == -1f || sel)
                    // {
                    //     hideSelectionRadius();
                    // } else {
                    //     showSelectionRadius();
                    // }
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
                } else {
                    if (Time.time - multiSelectTime > 0.1f)
                    {
                        multiSelect = false;
                        hideSelectionRadius();
                    } else {
                        showSelectionRadius();
                    }
                }

                if (squadState == SquadState.retreating)
                {
                    if (checkStopped(0.6f))
                        squadState = SquadState.stationary;
                }

                if (squadState == SquadState.moving || squadState == SquadState.attackmove)
                {
                    if (team == FogLayerManager.Instance.getPlayerTeam())
                        FogLayerManager.Instance.isDirty = true;
                    if (checkStopped(0.4f))
                        squadState = SquadState.stationary;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private bool checkStopped(float stopDistance)
        {
            foreach (GameObject obj in squadMembers)
            {
                if (obj != null)
                {
                    SquadMember squadMemberComponent = obj.GetComponent<SquadMember>();
                    if (squadMemberComponent.getRemainingDistance() > stopDistance)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Calls Unit select method and calls to show selection radiuses for this squad
        /// </summary>
        public override void select()
        {
            if (squadState != SquadState.retreating)
            {
                base.select();
                multiSelect = false;
                selected = true;
                showSelectionRadius();
            }
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
            if (squadState != SquadState.retreating)
            {
                Dictionary<GameObject, RaycastHit> movePositions = unitFunctions.getOptimalMovePositions(hits, squadMembers, squadLead);
                float longestMoveTime = getMaxMoveTime(movePositions, getSquadModelSpeed());
                foreach (var pair in movePositions)
                {
                    SquadMember component = pair.Key.GetComponent<SquadMember>();
                    // NavMeshAgent agent = pair.Key.GetComponent<NavMeshAgent>();
                    float distance = Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                    float speed = getNormalizedMoveSpeed(distance, longestMoveTime, getSquadModelSpeed());
                    component.moveToPosition(pair.Value.point, speed);
                }
                squadState = SquadState.moving;
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
            if (squadState != SquadState.retreating)
            {
                squadState = SquadState.retreating;
                Dictionary<GameObject, RaycastHit> movePositions = unitFunctions.getOptimalMovePositions(hits, squadMembers, squadLead);
                float longestMoveTime = getMaxMoveTime(movePositions, 10.0f);
                foreach (var pair in movePositions)
                {
                    SquadMember component = pair.Key.GetComponent<SquadMember>();
                    // NavMeshAgent agent = pair.Key.GetComponent<NavMeshAgent>();
                    float distance = Vector3.Distance(pair.Key.transform.position, pair.Value.point);
                    float speed = getNormalizedMoveSpeed(distance, longestMoveTime, getSquadModelSpeed());
                    component.moveToPosition(pair.Value.point, speed);
                }
            }
        }

        private bool killingModel = false;
        /// <summary>
        /// Function called by a squadMember object belonging to this squad when its
        /// health reaches 0. Allows the squad to always determine proper number of 
        /// alive members.
        /// </summary>
        /// <param name="modelObject"></param>
        public IEnumerator killModel(GameObject modelObject, SquadMember killComponent) 
        {
            while (killingModel)
                yield return null;
            killingModel = true;
            if (modelObject == squadLead)
                squadLead = null;
            squadMembers.Remove(modelObject);
            int modelIndex = checkLowestModelPriority(killComponent.modelPriority);
            if (modelIndex != -1)
            {
                Debug.Log("Lower model priority!");
                SquadMember cloneTarget = squadMembers[modelIndex].GetComponent<SquadMember>();
                reinforceMemberIndexes.Add(cloneTarget.prefabIndex);
                cloneModel(cloneTarget, killComponent);
            } else {
                reinforceMemberIndexes.Add(killComponent.prefabIndex);
            }
            Destroy(modelObject);
            if (FogLayerManager.Instance.getPlayerTeam() == team)
                FogLayerManager.Instance.isDirty = true;
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
            killingModel = false;
            yield break;
        }

        private int checkLowestModelPriority(int priority)
        {
            int lowestModelPriority = priority;
            int lowestPriorityIndex = -1;
            int i = 0;
            foreach(GameObject member in squadMembers)
            {
                SquadMember squadMemberComponent = member.GetComponent<SquadMember>();
                if (squadMemberComponent.modelPriority < lowestModelPriority)
                {
                    lowestModelPriority = squadMemberComponent.modelPriority;
                    lowestPriorityIndex = i;
                }
                i++;
            }
            return lowestPriorityIndex;
        }

        private void cloneModel(SquadMember cloneTarget, SquadMember cloneParent)
        {
            cloneTarget.prefabIndex = cloneParent.prefabIndex;
            cloneTarget.modelPriority = cloneParent.modelPriority;
            cloneTarget.setNewWeapon(cloneParent.getWeaponPrefab());
            cloneTarget.setMaxHealth(cloneParent.getMaxHealth());
            cloneTarget.setRange(cloneParent.getRange());
            cloneTarget.setDefense(cloneParent.getDefense());
            cloneTarget.setMoveSpeed(cloneParent.getMoveSpeed());
            cloneTarget.setModelAccuracyModifer(cloneParent.getModelAccuracyModifer());
            cloneTarget.setAttackSpeedModifier(cloneParent.getAttackSpeedModifier());
        }
        private void killSquad()
        {
            if (aliveMembers <= 0) {
                if (owner is Player)
                {
                    GameManager.Instance.player.GetPlayerUI().removeUnitIcon(uiIconComponent.gameObject);
                    GameManager.Instance.player.GetPlayerUI().removeWorldSpaceUnitIcon(worldIconComponent);
                    base.deselect();
                } else {
                    GameManager.Instance.player.GetPlayerUI().removeWorldSpaceUnitIcon(worldIconComponent);
                }
                Destroy(gameObject);
            }
        }

        // Fog of War Code

        // private IEnumerator fogLoop()
        // {
        //     bool previousRevealed = true;

        //     while (true)
        //     {
        //         bool currentRevealed = checkReveal();
                
        //         if (currentRevealed != previousRevealed)
        //         {
        //             revealStatus = currentRevealed;
        //             setSquadVisibility(currentRevealed);
        //             previousRevealed = currentRevealed;
        //         }

        //         yield return new WaitForSecondsRealtime(0.025f);
        //     }
        // }
        bool previousRevealed = true;
        private void fogCheck()
        {
            bool currentRevealed = checkReveal();
            
            if (currentRevealed != previousRevealed)
            {
                revealStatus = currentRevealed;
                setSquadVisibility(currentRevealed);
                previousRevealed = currentRevealed;
            }
        }
        
        public override bool checkReveal()
        {
            if (GameManager.Instance.player.team != team)
            {
                foreach (GameObject squadMember in squadMembers)
                {
                    if (squadMember != null)
                    {
                        SquadMember squadMemberComponent = squadMember.GetComponent<SquadMember>();
                        if (squadMemberComponent.fogDetection.visible)
                            return true;
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
                worldIconObj.SetActive(true);
                foreach (GameObject member in squadMembers)
                {
                    if (member != null)
                        member.GetComponent<SquadMember>().showModel();
                }       
            } else {
                worldIconObj.SetActive(false);
                foreach (GameObject member in squadMembers)
                {
                    if (member != null)
                        member.GetComponent<SquadMember>().hideModel();
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
                squadMemberComponent.prefabIndex = i;

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

            // Initializes the squad Icon and attaches it to the iconSprite empty object on the model.
            GameObject iconPosition = squadLead.transform.Find("iconPos").gameObject;
            if (iconPosition != null) {
                iconPosition.SetActive(true);

                worldIconComponent = GameManager.Instance.player.GetPlayerUI().spawnWorldSpaceUnitIcon(iconObj, iconPosition);
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
                    Player playerComponent = (Player)owner;
                    UnitIconUI uiIconComponent = playerComponent.GetPlayerUI().addNewUnitIcon(getPortrait(), getIcon(0));
                    uiIconComponent.setIconStatus(IconStatus.active);
                    setSquadIconUI(uiIconComponent);
                    uiIconComponent.setAliveModels(aliveMembers);
                    uiIconComponent.setHealtBarColor(0);
                    uiIconComponent.setCurrentHealth(1.0f);
                    uiIconComponent.setReferenceUnit(this);
                }
            }

            
            // StartCoroutine(fogLoop());
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
        public List<GameObject> getSquadModels() { return squadMembers; }
        public int getAliveMembers() { return aliveMembers; }

        public float getSquadCaptureRate() { return captureRate; }

        public Transform getCurrentTransform() {
            if (squadLead != null)
                return squadLead.transform;
            return null;
        }

        private float getSquadModelSpeed() {
            SquadMember component = squadMembers[0].GetComponent<SquadMember>();
            return component.getMoveSpeed();
        }
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

        public UnitIconUI getSquadIconUI() { return uiIconComponent; }

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

