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

        public void SquadSpawn(GameObject squadPrefab, Vector3 position, int team, int ownerTag, List<RaycastHit> rallyMove = null)
        {
            GameObject squadObj = Instantiate(squadPrefab, position, Quaternion.identity);
            Squad squad = squadObj.GetComponent<Squad>();
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
            SquadSpawn(Resources.Load<GameObject>("Prefabs/Units/Infantry Squads/ausRifleSquad"), new Vector3(60, 0, 60), 1, 1);
        }
    }
}
