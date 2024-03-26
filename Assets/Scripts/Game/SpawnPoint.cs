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
        public List<UnitData> buildQueue = new List<UnitData>();
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
                currentBuildTime = buildQueue[0].buildTime;
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
                    currentBuildTime = buildQueue[0].buildTime;
                }
            }
        }
        public void addToBuildQueue(UnitData unitData) 
        {
            if (buildQueue.Count < 3)
            {
                if (owner is Player)
                {
                    Player playerComponent = (Player)owner;
                    if (unitData is SquadData)
                    {
                        SquadData data = (SquadData)unitData;
                        unitIcons.Add(playerComponent.playerUI.addNewUnitIcon(data.portrait, data.icons[0]));
                    }
                    if (currentBuildTime <= 0.0f)
                        lastBuildTick = DateTime.Now;
                    buildQueue.Add(unitData);
                } else {
                    buildQueue.Add(unitData);
                }
            }
        }

        public void spawnUnit(UnitData unitData, UnitIconUI unitIconUI, RaycastHit rallyPoint = new RaycastHit())
        {
            if (unitData is SquadData)
            {
                SquadData squadData = (SquadData)unitData;
                GameObject squadObj = new GameObject(squadData.unitName, typeof(Squad));
                Squad squad = squadObj.GetComponent<Squad>();
                squad.squadData = squadData;
                squad.team = team;
                squad.setSquadIconUI(unitIconUI);
                GameActor component = gameManager.players[ownerTag].GetComponent<GameActor>();
                if (component != null)
                {
                    squad.owner = component;
                }
                squadObj.transform.position = transform.position;
            }
        }



        // -------- Getters / Setters ---------------

        public void setGameManager(GameManager instance)
        {
            gameManager = instance;
        }

        public Vector3 getCameraSpawnPosition()
        {
            return cameraSpawnPos.transform.position;
        }
    }
}
