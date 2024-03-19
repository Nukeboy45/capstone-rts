using System.Collections.Generic;
using UnityEngine;

namespace Capstone {
    public class SpawnPoint : MonoBehaviour
    {
        public GameActor owner;
        public FactionType faction;
        public int team;
        public int ownerTag;
        public RaycastHit rallyPoint;
        public List<UnitData> buildQueue = new List<UnitData>();
        public float currentBuildTime = 0.0f;
        private GameManager gameManager;

        public void Update()
        {
            if (buildQueue.Count != 0 && currentBuildTime <= 0.0f) 
            {
                currentBuildTime = buildQueue[0].buildTime;
            }

            if (currentBuildTime > 0.0f) 
            {
                currentBuildTime -= Time.deltaTime;
            }

            if (buildQueue.Count != 0 && currentBuildTime <= 0.0f) 
            {
                spawnUnit(buildQueue[0], rallyPoint);
                buildQueue.Remove(buildQueue[0]);   
            }
        }
        public void addToBuildQueue(UnitData unitData) 
        {
            buildQueue.Add(unitData);
        }

        public void spawnUnit(UnitData unitData, RaycastHit rallyPoint = new RaycastHit())
        {
            if (unitData is SquadData)
            {
                SquadData squadData = (SquadData)unitData;
                GameObject squadObj = new GameObject(squadData.unitName, typeof(Squad));
                Squad squad = squadObj.GetComponent<Squad>();
                squad.squadData = squadData;
                squad.team = team;
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
    }
}
