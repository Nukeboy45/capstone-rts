using UnityEngine;

namespace Capstone {
    [CreateAssetMenu(menuName = "Capstone/New Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int BurstSize;
        public float FiringCooldown;
        public float Accuracy;
        public float Damage;
    }
}
