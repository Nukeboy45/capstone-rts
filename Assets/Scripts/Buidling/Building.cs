using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone {

    public class Building : MonoBehaviour
    {
        public GameObject buildingObj;
        public float maxBuildingHealth;
        public int owner;
        public int team;
        public bool selected;
        private float currHealth;

        public void setCurrentHealth(float health) {
            currHealth = health;
        }

        public float getCurrentHealth() {
            return currHealth;
        }
    }
}