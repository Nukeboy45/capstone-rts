using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone
{
    public class PassiveBuilding : Building
    {
        public PassiveBuildingData data;
        public bool selected;
        private float health;
        private Vector3 rallyPoint;
        private bool canProduce;
        void Start() 
        {
            health = data.buildingHealth;
        }
    }
}