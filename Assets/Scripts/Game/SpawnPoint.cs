using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Capstone {
    public class SpawnPoint : MonoBehaviour
    {
        public GameActor owner;
        public FactionType faction;
        public int team;
        public int ownerTag;
        public RaycastHit rallyPoint;
        public GameObject cameraSpawnPos;
        public List<GameObject> buildQueue = new List<GameObject>();
        private List<UnitIconUI> unitIcons = new List<UnitIconUI>();
        private GameManager gameManager;

        // Timing Variables
        private DateTime lastBuildTick;
        private float currentBuildTime = 0.0f;


        public void Start()
        {

        }

        public void Update()
        {
            TimeSpan elapsedBuildTime = DateTime.Now - lastBuildTick;
            if (buildQueue.Count != 0 && currentBuildTime <= 0.0f) 
            {
                currentBuildTime = buildQueue[0].GetComponent<Unit>().getBuildTime();
            }

            if (buildQueue.Count != 0 && elapsedBuildTime.Seconds < currentBuildTime)
            {
                if (unitIcons[0].getHealthBar() != null)
                {
                    float timeElapsedFloat = (float)elapsedBuildTime.Seconds + (float)elapsedBuildTime.Milliseconds / 1000f;
                    float currentBuildProgress = timeElapsedFloat / currentBuildTime;
                    unitIcons[0].setCurrentHealth(currentBuildProgress);
                }
            }

            if (buildQueue.Count != 0 && elapsedBuildTime.Seconds >= currentBuildTime) 
            {
                unitIcons[0].setIconStatus(IconStatus.active);
                spawnUnit(buildQueue[0], unitIcons[0], rallyPoint);
                buildQueue.Remove(buildQueue[0]);   
                unitIcons.Remove(unitIcons[0]);
                if (buildQueue.Count == 0)
                {
                    currentBuildTime = 0.0f;
                } else {
                    lastBuildTick = DateTime.Now;
                    unitIcons[0].setIconStatus(IconStatus.building);
                    currentBuildTime = buildQueue[0].GetComponent<Unit>().getBuildTime();
                }
            }
        }
        public void addToBuildQueue(GameObject unitPrefab) 
        {
            if (buildQueue.Count < 3)
            {
                Unit unitComponent = unitPrefab.GetComponent<Unit>();
                if (owner is Player)
                {
                    Player playerComponent = (Player)owner;
                    UnitIconUI newIcon = playerComponent.GetPlayerUI().addNewUnitIcon(unitComponent.getPortrait(), unitComponent.getIcon(0));
                    newIcon.setSpawnPoint(this);
                    unitIcons.Add(newIcon);
                    if (currentBuildTime <= 0.0f)
                        lastBuildTick = DateTime.Now;
                    buildQueue.Add(unitPrefab);
                } else {
                    buildQueue.Add(unitPrefab);
                }
            }
        }

        public void spawnUnit(GameObject unitPrefab, UnitIconUI unitIconUI, RaycastHit rallyPoint = new RaycastHit())
        {
            if (unitPrefab.GetComponent<Squad>() != null)
            {
                GameObject squadObj = Instantiate(unitPrefab, transform.position, Quaternion.identity);
                Squad squad = squadObj.GetComponent<Squad>();
                squad.setSquadIconUI(unitIconUI);
                squad.team = team;
                GameActor component = gameManager.players[ownerTag].GetComponent<GameActor>();
                if (component != null)
                {
                    squad.owner = component;
                }
                squadObj.transform.position = transform.position;
                if (!rallyPoint.Equals(default(RaycastHit)))
                {
                    List<RaycastHit> hits = Selection.getAdditionalCasts(rallyPoint, GameManager.Instance.rayCamera, transform, squad.getAliveMembers(), LayerMask.NameToLayer("Ground"));
                }
            }
        }

        public void removeFromBuildQueue(UnitIconUI removeIcon)
        {
            if (unitIcons[0] == removeIcon)
                lastBuildTick = DateTime.Now;
            GameManager.Instance.player.GetPlayerUI().removeUnitIcon(removeIcon.gameObject);
            int index = unitIcons.IndexOf(removeIcon);
            buildQueue.RemoveAt(index);
            unitIcons.Remove(removeIcon);
        }



        // -------- Getters / Setters ---------------

        public void setGameManager(GameManager instance)
        {
            gameManager = instance;
        }

        public Vector3 getCameraSpawnPosition() { return cameraSpawnPos.transform.position; }
    }
}
