using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstone {

    public class Building : MonoBehaviour
    {
        public BuildingData buildingData;
        public GameObject buildingObj;
        public float maxBuildingHealth;
        public float currHealth;
        public int owner;
        public int team;
    }
}