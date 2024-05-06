using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Capstone {
    public class FogDetection : MonoBehaviour
    {
        [SerializeField] private Collider visualCollider;
        public bool visible = false;
        public List<Collider> colliders = new List<Collider>();

        void Update() {
            colliders.RemoveAll(c => c == null);
            if (colliders.Count == 0 && visible != false)
                visible = false;
        }
        void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("FOV") && !colliders.Contains(other)) {
                colliders.Add(other);
                UpdateVisibility();
            }
        }

        void OnTriggerStay(Collider other) {
            if (other == visualCollider && !colliders.Contains(other))
            {
                colliders.Add(other);
            }
        }

        void OnTriggerExit(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("FOV") && colliders.Contains(other)) {
                colliders.Remove(other);
                UpdateVisibility();
            }
        }

        void UpdateVisibility() {
            visible = colliders.Count > 0;
        }
    }
}
