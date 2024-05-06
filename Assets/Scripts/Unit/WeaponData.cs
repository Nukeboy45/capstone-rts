using UnityEngine;

namespace Capstone {
    [CreateAssetMenu(menuName = "Capstone/New Weapon")]
    public class WeaponData : ScriptableObject
    {
        public int MagazineCapacity;
        public float FiringCooldown;
        public float ReloadTime;
        public float MinAimTime;
        public float MaxAimTime;
        public float MaxAccuracy;
        public float MaxAccuracyRange;
        public float AccuracyFalloffFactor;
        public float AccuracyFalloffDistance;
        public float Damage;
        public AudioClip[] weaponSounds; // 0 is firing, 1 is reloading, 2+ are weapon-specific
    }
}
