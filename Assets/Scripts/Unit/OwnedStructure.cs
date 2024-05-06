using UnityEngine;

namespace Capstone 
{
    public class OwnedStructure : MonoBehaviour
    {
        [Header("Gameplay Structure Variables - Determined at Runtime")]
        public int team;
        public GameActor owner;
        public bool? showSelect = null;
        public bool selected = false;
        public BuildingType buildingType;

        [Header("Default Structure Variables - shared by all children")]
        [SerializeField] protected GameObject iconObj;
        [SerializeField] protected Sprite iconSprites;
        [SerializeField] protected float buildTime;
        [SerializeField] protected float health;
        [SerializeField] protected float smallArmsDamageResist;
        [SerializeField] protected float explosiveDamageResist;
    }
}