using UnityEngine;

namespace Capstone
{
    public class HideObject : MonoBehaviour
    {
        private int showLayer = 3;
        private int hideLayer = 10;

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