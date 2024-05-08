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
            GameObject squadPrefab;
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
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(90, 0, 90), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(105, 0, 105), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(183, 0, 183), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(198, 0, 198), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(294, 0, 294), 1, 1));
            handle = Addressables.LoadAssetAsync<GameObject>("entRifleSquad");
            yield return StartCoroutine(SquadSpawn(handle, new Vector3(282, 0, 282), 1, 1));
        }
    }
}
