using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Capstone
{
    public class UnitData : ScriptableObject
    {
        public string unitName;
        public string desc;
        public GameObject iconObj;
        public Sprite[] icons;
        public Sprite icon;
    }

}