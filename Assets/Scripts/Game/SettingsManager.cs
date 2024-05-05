using System.Collections;
using System.Collections.Generic;
using System.IO;
using Capstone;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public bool isLoaded = false;
    public GameSettings settings;

    void Awake()
    {
        createSettingsManagerInstance();
    }

    void createSettingsManagerInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
            Destroy(gameObject);
        loadSavedSettings();
    }

    public void loadSavedSettings()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "settings.json");
        if (File.Exists(filePath))
        {
            settings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(filePath));
            Screen.SetResolution(settings.resWidth, settings.resHeight, settings.fullscreen);
        } else {
            settings = new GameSettings();
            settings.mainVolume = 100;
            settings.musicVolume = 100;
            settings.resWidth = 3840;
            settings.resHeight = 2160;
            settings.fullscreen = true;
            saveSettings();
            Screen.SetResolution(settings.resWidth, settings.resHeight, settings.fullscreen);
        }
        isLoaded = true;
    }

    public void updateScreenResolution(int width, int height, bool fullscreen, bool save)
    {
        settings.resWidth = width;
        settings.resHeight = height;
        settings.fullscreen = fullscreen;
        if (save)
            saveSettings();
    }

    public void updateMainVolume(int volume)
    {
        settings.mainVolume = volume;
        saveSettings();
    }

    public void updateMusicVolume(int volume)
    {
        settings.musicVolume = volume;
        saveSettings();
    }

    public void saveSettings()
    {
        Debug.Log("Savings settings: " + settings);
        string json = JsonUtility.ToJson(settings);
        string filePath = Path.Combine(Application.persistentDataPath, "settings.json");
        File.WriteAllText(filePath, json);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
