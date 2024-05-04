using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Capstone {
    public class FogDetection : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider visualCollider;
        public bool unitVisible = false;
        public List<Collider> colliders = new List<Collider>();

        void Update() {
            colliders.RemoveAll(c => c == null);
            if (colliders.Count == 0 && unitVisible != false)
                unitVisible = false;
        }
        void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("FOV") && !colliders.Contains(other)) {
                colliders.Add(other);
            }
            UpdateVisibility();
        }

        void OnTriggerExit(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("FOV") && colliders.Contains(other)) {
                colliders.Remove(other);
            }
            UpdateVisibility();
        }

        void UpdateVisibility() {
            unitVisible = colliders.Count > 0;
        }
    }
}
