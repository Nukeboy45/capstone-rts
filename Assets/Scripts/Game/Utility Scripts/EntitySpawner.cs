using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public static class EntitySpawner
    {
        /*public static void SpawnBuilding(BuildingData buildingData, Vector3 position, int team, int ownerTag)
        {
            if (buildingData is PassiveBuildingData) {

            } else if (buildingData is ActiveBuildingData) {

            }
        }*/

        public static void SquadSpawn(SquadData squadData, Vector3 position, int team, int ownerTag)
        {
            GameObject squadObj = new GameObject(squadData.unitName);
            Squad squad = squadObj.AddComponent<Squad>();
            squad.squadData = squadData;
            squad.team = team;
            GameActor component = GameManager.Instance.players[ownerTag].GetComponent<GameActor>();
            if (component != null)
            {
                squad.owner = component;
            }
            
            squad.transform.position = position;
        }
    }
}
