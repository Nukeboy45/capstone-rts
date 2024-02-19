using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class PassiveBuilding : Building
    {
        public PassiveBuildingData data;
        private Vector3 rallyPoint;
        private bool canReinforce;
        private bool canHeal;
        private float healRange;
        private GameObject[] productionList;

        void Start() 
        {
            InstantiatePassiveBuilding();
        }

        void InstantiatePassiveBuilding() {
            // Initializing the buildingObj 3d model and settings its transform to the PassiveBuildingObj
            //buildingObj = Instantiate(data.model, this.transform.position, Quaternion.identity);
            //buildingObj.transform.parent = this.transform;

            // Generic building variables
            maxBuildingHealth = data.buildingHealth;
            setCurrentHealth(data.buildingHealth);

            // Passive building variables
            canReinforce = data.canReinforce;
            canHeal = data.canHeal;
            if (data.productionList != null) {
                productionList = data.productionList;
            }
        }

        public override void select() 
        {
            base.select();
        }

        private void SpawnUnit(GameObject unit) 
        {

        }
        
        public void setRallyPoint(RaycastHit hit)
        {

        }
    }
}