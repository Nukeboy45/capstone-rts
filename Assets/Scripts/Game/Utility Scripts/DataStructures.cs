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
    public class GameSettings
    {
        public int mainVolume;
        public int musicVolume;
        public int resWidth;
        public int resHeight;
        public bool fullscreen;
    }

    [System.Serializable]
    public class MapData
    {
        public string mapID;
        public string mapName;
        public int playerSlots;
        public int mapSize;
    }
}