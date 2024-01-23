using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class PassiveBuilding : Building
    {
        public PassiveBuildingData data;
        public bool selected;
        private Vector3 rallyPoint;
        private bool canReinforce;
        private bool canHeal;
        private float healRange;
        private UnitData[] productionList;
        void Start() 
        {
            InstantiatePassiveBuilding();
        }

        void InstantiatePassiveBuilding() {
            // Initializing the buildingObj 3d model and settings its transform to the PassiveBuildingObj
            buildingObj = Instantiate(data.model, this.transform.position, Quaternion.identity);
            buildingObj.transform.parent = this.transform;

            // Generic building variables
            maxBuildingHealth = data.buildingHealth;
            currHealth = data.buildingHealth;
            team = data.team;
            owner = data.owner;

            // Passive building variables
            canReinforce = data.canReinforce;
            canHeal = data.canHeal;
            productionList = data.productionList;
        }
    }
}