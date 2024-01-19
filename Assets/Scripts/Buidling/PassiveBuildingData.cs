using UnityEngine;
namespace Capstone
{
    [CreateAssetMenu(menuName = "Capstone/New Passive Building")]
    public class PassiveBuildingData : BuildingData
    {
        public bool canReinforce;
        public bool canHeal;
        public UnitData[] productionList;
        public float healRange;
    }

}