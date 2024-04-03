using UnityEngine;

namespace Capstone
{
    public class WeaponAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        private AudioClip[] weaponSounds;

        public void PlaySound(int index)
        {
            if (index >= 0 && index < weaponSounds.Length)
            {
                if (weaponSounds[index] != null && !audioSource.isPlaying)
                    audioSource.clip = weaponSounds[index];
                    audioSource.Play();
            }
        }

        // --- Getter / Setter Methods ---

        public void setWeaponSounds(AudioClip[] sounds) { weaponSounds = sounds; }

    }
}