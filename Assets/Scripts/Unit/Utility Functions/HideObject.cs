using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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