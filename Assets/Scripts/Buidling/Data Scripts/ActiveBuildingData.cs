using UnityEngine;
namespace Capstone
{
    [CreateAssetMenu(menuName = "Capstone/New Active Building")]
    public class ActiveBuildingData : BuildingData
    {
        public float range;
        public float damage;
        public float attackSpeed;
    }
}