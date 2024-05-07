using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Capstone 
{
    public class ProductionStructure : OwnedStructure
    {
        [Header("Production Structure Variables")]
        [SerializeField] public GameObject[] constructableUnits = new GameObject[4];
        [SerializeField] protected GameObject spawnPoint;

        // Private Runtime Variables
        private List<GameObject> buildQueue = new List<GameObject>();
        private List<UnitIconUI> buildQueueIcons = new List<UnitIconUI>();
        private RaycastHit rallyPoint;

        // Timing Variables
        private DateTime lastBuildTick;
        private float currentBuildTime = 0.0f;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            TimeSpan elapsedBuildTime = DateTime.Now - lastBuildTick;
            if (buildQueue.Count != 0 && currentBuildTime <= 0.0f) 
            {
                currentBuildTime = buildQueue[0].GetComponent<Unit>().getBuildTime();
            }

            if (buildQueue.Count != 0 && elapsedBuildTime.Seconds < currentBuildTime)
            {
                if (buildQueueIcons[0].getHealthBar() != null)
                {
                    float timeElapsedFloat = (float)elapsedBuildTime.Seconds + (float)elapsedBuildTime.Milliseconds / 1000f;
                    float currentBuildProgress = timeElapsedFloat / currentBuildTime;
                    buildQueueIcons[0].setCurrentHealth(currentBuildProgress);
                }
            }

            if (buildQueue.Count != 0 && elapsedBuildTime.Seconds >= currentBuildTime) 
            {
                buildQueueIcons[0].setIconStatus(IconStatus.active);
                buildQueueIcons[0].eliminateProductionReference();
                spawnUnit(buildQueue[0], buildQueueIcons[0], rallyPoint);
                buildQueue.Remove(buildQueue[0]);   
                buildQueueIcons.Remove(buildQueueIcons[0]);
                if (buildQueue.Count == 0)
                {
                    currentBuildTime = 0.0f;
                } else {
                    lastBuildTick = DateTime.Now;
                    buildQueueIcons[0].setIconStatus(IconStatus.building);
                    currentBuildTime = buildQueue[0].GetComponent<Unit>().getBuildTime();
                }
            }
        }
        public void addToBuildQueue(int constructableUnitIndex) 
        {
            Debug.Log("Called!");
            if (buildQueue.Count < 3)
            {
                GameObject prefab = constructableUnits[constructableUnitIndex];
                if (prefab != null)
                {
                    Unit unitComponent = prefab.GetComponent<Unit>();
                    if (owner is Player)
                    {
                        Player playerComponent = (Player)owner;
                        UnitIconUI newIcon = playerComponent.GetPlayerUI().addNewUnitIcon(unitComponent.getPortrait(), unitComponent.getIcon(0));
                        newIcon.setProductionStructure(this);
                        buildQueueIcons.Add(newIcon);
                        if (currentBuildTime <= 0.0f)
                            lastBuildTick = DateTime.Now;
                        buildQueue.Add(prefab);
                    } else {
                        buildQueue.Add(prefab);
                    }
                }
            }
        }

        public void spawnUnit(GameObject unitPrefab, UnitIconUI unitIconUI, RaycastHit rallyPoint = new RaycastHit())
        {
            if (unitPrefab.GetComponent<Squad>() != null)
            {
                GameObject squadObj = Instantiate(unitPrefab, spawnPoint.transform.position, Quaternion.identity);
                Squad squad = squadObj.GetComponent<Squad>();
                squad.setSquadIconUI(unitIconUI);
                squad.team = team;
                squad.owner = owner;
                //squadObj.transform.position = transform.position;
                if (!rallyPoint.Equals(default(RaycastHit)))
                {
                    List<RaycastHit> hits = Selection.getAdditionalCasts(rallyPoint, GameManager.Instance.rayCamera, transform, squad.getAliveMembers(), LayerMask.NameToLayer("Ground"));
                }
            }
        }

        public void removeFromBuildQueue(UnitIconUI removeIcon)
        {
            if (buildQueueIcons[0] == removeIcon)
                lastBuildTick = DateTime.Now;
            GameManager.Instance.player.GetPlayerUI().removeUnitIcon(removeIcon.gameObject);
            int index = buildQueueIcons.IndexOf(removeIcon);
            buildQueue.RemoveAt(index);
            buildQueueIcons.Remove(removeIcon);
        }

        public GameObject getSpawnPoint() { return spawnPoint; }
    }
}

