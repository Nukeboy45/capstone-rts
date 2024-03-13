using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using Capstone;

public class SkirmishMenu : MonoBehaviour
{
    public Transform mapListContent;
    private List<MapData> maps = new List<MapData>();

    // Start is called before the first frame update
    void Start()
    {
        string[] sceneNames = Directory.GetFiles("Assets/Scenes/SkirmishMaps", "*.unity");

        // Loading all maps in the "Skirmish Maps" Folder
        foreach (string name in sceneNames)
        {
            string mapName = Path.GetFileNameWithoutExtension(name);

            TextAsset mapJson = Resources.Load<TextAsset>("Maps/" + mapName);

            if (mapJson == null)
            {
                Debug.Log("Failed to load map data!");
            } else {
                MapData mapData = JsonUtility.FromJson<MapData>(mapJson.text);
                maps.Add(mapData);
            }
        }


    }
}
