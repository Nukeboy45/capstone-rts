using UnityEngine;
namespace Capstone
{
    [CreateAssetMenu(menuName = "Capstone/New Defensive Building")]
    public class DefenseBuildingData : BuildingData
    {
        public float range;
        public float damage;
        public float attackSpeed;
    }
}