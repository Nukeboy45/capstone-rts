using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class EntitySpawner : MonoBehaviour
    {
        /*public static void SpawnBuilding(BuildingData buildingData, Vector3 position, int team, int ownerTag)
        {
            if (buildingData is PassiveBuildingData) {

            } else if (buildingData is ActiveBuildingData) {

            }
        }*/
        [SerializeField] private GameManager gameManager;

        public void Start()
        {
            StartCoroutine(spawnDebugEnemySquad());
        }

        public void SquadSpawn(SquadData squadData, Vector3 position, int team, int ownerTag, List<RaycastHit> rallyMove = null)
        {
            GameObject squadObj = new GameObject(squadData.unitName);
            Squad squad = squadObj.AddComponent<Squad>();
            squad.squadData = squadData;
            squad.team = team;
            GameActor component = gameManager.players[ownerTag].GetComponent<GameActor>();
            if (component != null)
            {
                squad.owner = component;
            }
            squadObj.transform.position = position;
            if (rallyMove != null)
            {
                squad.moveTo(rallyMove);
            }
        }

        private IEnumerator spawnDebugEnemySquad()
        {
            while (gameManager.players.Length < 2)
            {
                yield return null;
            }
            SquadSpawn(Resources.Load<SquadData>("Units/Austrian/Infantry/ausRifle"), new Vector3(60, 0, 60), 1, 1);
        }
    }
}
