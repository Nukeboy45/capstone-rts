using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        public IEnumerator SquadSpawn(AsyncOperationHandle squadAsyncOperation, Vector3 position, int team, int ownerTag, List<RaycastHit> rallyMove = null)
        {
            GameObject squadPrefab = null;
            yield return squadAsyncOperation;
            if (squadAsyncOperation.Status == AsyncOperationStatus.Succeeded)
            {
                squadPrefab = squadAsyncOperation.Result as GameObject;
            }
            else 
            {
                yield break;
            }
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
            while (GameManager.Instance.players.Length < 2 || GameManager.Instance.playerUI == null)
            {
                yield return null;
            }

            AsyncOperationHandle handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(60, 0, 60), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("cenRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(40, 0, 40), 0, 0));
        }
    }
}
