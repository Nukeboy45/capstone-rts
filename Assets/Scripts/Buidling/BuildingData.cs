using UnityEngine;
namespace Capstone
{
    public class BuildingData : ScriptableObject
    {
        public string buildingName;
        public BuildingType type;
        public GameObject model;
        public float buildingHealth;
        public int owner;
        public int team;
    }
}