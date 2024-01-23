using UnityEngine;

namespace Capstone
{
    public static class EntitySpawner
    {
        public static void SpawnPassiveBuilding(PassiveBuildingData buildingData, Vector3 position, int team, int ownerTag)
        {
            /*buildingData.team = team;
            buildingData.owner = ownerTag;
            GameObject hqObj = new GameObject(buildingData.buildingName);
            hq.data = buildingData;
            hq.buildingObj = hqObj;

            hq.transform.position = position;*/
        }

        public static void SquadSpawn(SquadData squadData, Vector3 position, int team, int ownerTag)
        {
            squadData.team = team;
            squadData.owner = ownerTag;
            GameObject squadObj = new GameObject(squadData.unitName);
            Squad squad = squadObj.AddComponent<Squad>();
            squad.squadData = squadData;
            squad.unitObj = squadObj;
            
            squad.transform.position = position;
        }
    }
}
