using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Capstone 
{
    [System.Serializable]
    public class ScoreBarList
    {
        public Slider sliderComponent;
        public TextMeshProUGUI textComponent;
    }

    [System.Serializable]
    public class UnitIconUIWorldPair
    {
        public GameObject icon;
        public GameObject position;
    }
}