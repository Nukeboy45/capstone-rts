using UnityEngine;

namespace Capstone {
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private int magazineCapacity;
        [SerializeField] private float firingCooldown;
        [SerializeField] private float reloadTime;
        [SerializeField] private float minAimTime;
        [SerializeField] private float maxAimTime;
        [SerializeField] private float maxAccuracy;
        [SerializeField] private float maxAccuracyRange;
        [SerializeField] private float accuracyFalloffFactor;
        [SerializeField] private float accuracyFalloffDistance;
        [SerializeField] private float damage;
        [SerializeField] private AudioClip[] weaponSounds; // 0 is firing, 1 is reloading, 2+ are weapon-specific
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem muzzleSmoke;

        public void shootWeapon()
        {
            muzzleFlash.Play();
            muzzleSmoke.Play();
        }

        public int getMagazineCapacity() { return magazineCapacity; }
        public float getFiringCooldown() { return firingCooldown; }
        public float getReloadTime() { return reloadTime; }
        public float getMinAimTime() { return minAimTime; }
        public float getMaxAimTime() { return maxAimTime; }
        public float getMaxAccuracy() { return maxAccuracy; }
        public float getMaxAccuracyRange() { return maxAccuracyRange; }
        public float getAccuracyFalloffFactor() { return accuracyFalloffFactor; }
        public float getAccuracyFalloffDistance() { return accuracyFalloffDistance; }
        public float getDamage() { return damage;}
        public AudioClip[] getWeaponSounds() { return weaponSounds; }
    }
}