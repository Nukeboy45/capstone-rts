using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        string filePath = Path.Combine(Application.streamingAssetsPath, "settings.json");
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
            settings.vsync = true;
            saveSettings();
            Screen.SetResolution(settings.resWidth, settings.resHeight, settings.fullscreen);
        }
        printCurrentSettings();
        isLoaded = true;
    }

    public void updateScreenSettings(int width, int height, bool fullscreen, bool vsync, bool save)
    {
        settings.resWidth = width;
        settings.resHeight = height;
        settings.fullscreen = fullscreen;
        settings.vsync = vsync;
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

    public async void saveSettings()
    {
        try {
            printCurrentSettings();
            string json = JsonUtility.ToJson(settings);
            string filePath = Path.Combine(Application.streamingAssetsPath, "settings.json");
            await Task.Run(() => File.WriteAllText(filePath, json));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to write settings " + e.Message);
        }
    }

    private void printCurrentSettings()
    {
        Debug.Log(settings.mainVolume);
        Debug.Log(settings.musicVolume);
        Debug.Log(settings.resWidth);
        Debug.Log(settings.resHeight);
        Debug.Log(settings.fullscreen);
        Debug.Log(settings.vsync);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
