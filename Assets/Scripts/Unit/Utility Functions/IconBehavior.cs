using UnityEngine;

namespace Capstone 
{
    public class IconBehavior : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}