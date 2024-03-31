using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Capstone
{
    public class GetParentIcon : MonoBehaviour
    {
        [SerializeField] private GameObject iconObj;
    
        public GameObject getParentIcon()
        {
            return iconObj;
        }
    }
}