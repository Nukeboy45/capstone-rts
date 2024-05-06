using UnityEngine;

namespace Capstone 
{
    public class ProductionStructure : OwnedStructure
    {
        [Header("Production Structure Variables")]
        [SerializeField] protected GameObject[] constructableUnits = new GameObject[4];
        [SerializeField] protected GameObject spawnPoint;

        // Private Runtime Variables
        private RaycastHit rallyPoint;
    }
}

