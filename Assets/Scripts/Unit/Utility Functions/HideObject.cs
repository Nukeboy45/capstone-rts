using UnityEngine;

namespace Capstone
{
    public class HideObject : MonoBehaviour
    {
        [SerializeField] private int showLayer;
        [SerializeField] private int hideLayer;

        public void showObj() {
            gameObject.layer = showLayer;
        }

        public void hideObj() {
            gameObject.layer = hideLayer;
        }
    }
}