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

        public void Start()
        {
            StartCoroutine(spawnDebugSquads());
        }

        public void SquadSpawn(GameObject squadPrefab, Vector3 position, int team, int ownerTag, List<RaycastHit> rallyMove = null)
        {
            GameObject squadObj = Instantiate(squadPrefab, position, Quaternion.identity);
            Squad squad = squadObj.GetComponent<Squad>();
            squad.team = team;
            GameActor component = GameManager.Instance.players[ownerTag].GetComponent<GameActor>();
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

        private IEnumerator spawnDebugSquads()
        {
            while (GameManager.Instance.players.Length < 2 || GameManager.Instance.playerUIReference == null)
            {
                yield return null;
            }
            SquadSpawn(Resources.Load<GameObject>("Prefabs/Units/Infantry Squads/ausRifleSquad"), new Vector3(60, 0, 60), 1, 1);
            SquadSpawn(Resources.Load<GameObject>("Prefabs/Units/Infantry Squads/ausRifleSquad"), new Vector3(50, 0, 50), 0, 0);
        }
    }
}
