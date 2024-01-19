using UnityEngine;

namespace Capstone
{
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public UnitType unitType;
        public string description;
        public GameObject icon;
        public int owner;
        public int team;
    }

}