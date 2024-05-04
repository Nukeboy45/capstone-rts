using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Capstone;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkirmishMenu : MonoBehaviour
{
    // Menu Elements
    public Transform mapListContent;
    public GameObject mapOptions;
    public GameObject[] slotOptions;
    public Button startButton;

    // Logic / Runtime Variables
    private string selectedMap = null;
    private int selectedMapSlots = 0;
    private Button selectedMapButton = null;
    private List<MapData> maps = new List<MapData>();

    // Start is called before the first frame update
    void Start()
    {
        importAllMapData();
        initializeMapList();
    }

    void Update()
    {
    }

    public void importAllMapData()
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

    public void initializeMapList()
    {
        float offset = -110f;
        int times = 0;
        foreach (MapData data in maps)
        {
            // Initializing the map selection button and adding its name
            GameObject mapButton = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MainMenu/mapElementPrefab"), mapListContent);
            TextMeshProUGUI mapName = mapButton.transform.Find("mapName").GetComponent<TextMeshProUGUI>();
            mapName.text = "  " + data.mapName;
            TextMeshProUGUI mapSlots = mapButton.transform.Find("mapSlots").GetComponent<TextMeshProUGUI>();
            mapSlots.text = "(" + data.playerSlots.ToString() + ")";
            mapButton.transform.position += new Vector3(5, -10 + offset * times, 0);

            //
            Button mapButtonComponent = mapButton.GetComponent<Button>();
            mapButtonComponent.onClick.AddListener(() => selectMap(data.mapID, data.playerSlots, mapButtonComponent));

            times += 1;
        }
    }

    public void selectMap(string mapName, int mapSlots, Button selectedButton)
    {
        if (selectedButton != selectedMapButton)
        {
            if (selectedMapButton != null)
            {
                selectedMapButton.GetComponent<Image>().color = Color.white;
            }
            Color color = new Color(0.7f,0.7f,0.7f,1.0f);
            selectedButton.GetComponent<Image>().color = color;
            selectedMap = mapName;
            selectedMapSlots = mapSlots;
            selectedMapButton = selectedButton;
            startButton.interactable = true;
            TextMeshProUGUI startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            startButtonText.color = new Color(255, 255, 255, 255);
        } else {
            if (selectedMapButton != null)
            {
                selectedMapButton.GetComponent<Image>().color = Color.white;
            }
            selectedMap = null;
            selectedMapSlots = mapSlots;
            selectedMapButton = null;
            startButton.interactable = false;
            TextMeshProUGUI startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            startButtonText.color = new Color(255, 255, 255, 100);
        }
        updateSelectionMenu(mapSlots);
    }

    public void startGame()
    {
        int team1Members = 0;
        int team2Members = 0;
        int playerCount = 0;
        for (int i=0; i < selectedMapSlots; i++) 
        {
            if (slotOptions[i].GetComponent<MapSlot>().getTeam() == 0)
            {
                team1Members++;
            } else {
                team2Members++;
            }

            if (slotOptions[i].GetComponent<MapSlot>().getPlayerType() == GameActorType.player)
            {
                playerCount++;
            }
        }

        if (checkStartConditions(team1Members, team2Members, playerCount))
        {
            MatchManager.instance.resetMatchVariables();
            for (int i=0; i < selectedMapSlots; i++)
            {
                MapSlot slot = slotOptions[i].GetComponent<MapSlot>();
                MatchManager.instance.addMatchMember(slot.getTeam(), slot.getPlayerType(), slot.getFaction());
            }
            SceneManager.LoadScene(selectedMap);
        } else {
            Debug.Log("Cannot start match!");
        }
    }

    private void updateSelectionMenu(int mapSlots)
    {
        if (selectedMapButton != null)
        {
            mapOptions.SetActive(true);
            for (int i=0; i < slotOptions.Length; i++)
            {
                slotOptions[i].SetActive(false);
            }
            for (int i=0; i < mapSlots; i++)
            {
                slotOptions[i].SetActive(true);
            }
        } else {
            mapOptions.SetActive(false);
        }
    }

    private bool checkStartConditions(int team1Members, int team2Members, int playerCount)
    {
        
        if (team1Members == 0 || team2Members == 0)
        {
            Debug.Log("Cannot start a match without at least 1 member on both teams!");
            return false;
        } 
        else if (team1Members > selectedMapSlots / 2 || team2Members > selectedMapSlots / 2)
        {
            Debug.Log("Too many players on one side!");
            return false;
        } 
        else if (playerCount > 1)
        {
            Debug.Log("Cannot have more than one human player, not multiplayer yet :(");
            return false;
        }
        else if (playerCount == 0)
        {
            Debug.Log("Must be at least one human player!");
            return false;
        }
        else {
            return true;
        }
    }
}
