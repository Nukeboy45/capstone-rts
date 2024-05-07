using UnityEngine;

namespace Capstone
{
    public class HideObject : MonoBehaviour
    {
        [SerializeField] private int showLayer;
        [SerializeField] private int hideLayer;

        public void showObj() {
            changeAllChildren(transform, showLayer);
        }

        public void hideObj() {
            changeAllChildren(transform, hideLayer);
        }

        private void changeAllChildren(Transform parent, int newLayer)
        {
            parent.gameObject.layer = newLayer;
            foreach(Transform child in parent)
                changeAllChildren(child, newLayer);
        }
    }
}