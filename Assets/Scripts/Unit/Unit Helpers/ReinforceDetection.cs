using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Capstone {
    public class ReinforceDetection : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider visualCollider;
        public bool unitReinforceable = false;
        public List<Collider> colliders = new List<Collider>();

        void Update() {
            colliders.RemoveAll(c => c == null);
            if (colliders.Count == 0 && unitReinforceable != false)
                unitReinforceable = false;
        }
        void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("ReinforceRadius") && !colliders.Contains(other)) {
                colliders.Add(other);
                UpdateReinforce();
            }
        }

        void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("ReinforceRadius") && colliders.Contains(other)) {
                colliders.Remove(other);
                UpdateReinforce();
            }
        }

        void UpdateReinforce() {
            unitReinforceable = colliders.Count > 0;
        }
    }
}
