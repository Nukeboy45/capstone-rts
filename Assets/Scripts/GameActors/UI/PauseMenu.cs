using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Capstone {
    public class PauseMenu : MonoBehaviour
    {
        [Header("Main Volume Bar")]
        [SerializeField] private Slider mainVolumeBar;
        [SerializeField] private TextMeshProUGUI mainVolumeValueLabel;

        [Header("Music Volume Bar")]
        [SerializeField] private Slider musicVolumeBar;
        [SerializeField] private TextMeshProUGUI musicVolumeValueLabel;

        [Header("Screen Settings")]
        [SerializeField] private TMP_Dropdown resolutionSelect;
        [SerializeField] private Toggle fullscreenCheckbox;
        [SerializeField] private Toggle vsyncToggle;

        [Header("Pause Menu GameObjects")]
        [SerializeField] private GameObject resumeButton;
        [SerializeField] private GameObject settingsButton;
        [SerializeField] private GameObject quitButton;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private GameObject settingsContainer;

        void Start()
        {
            mainVolumeBar.onValueChanged.AddListener((value) => onVolumeValueChange(value, "main"));
            musicVolumeBar.onValueChanged.AddListener((value) => onVolumeValueChange(value, "music"));
            StartCoroutine(initializeLoadedSettings());
        }

        private IEnumerator initializeLoadedSettings()
        {
            while (SettingsManager.Instance.isLoaded == false)
            { yield return null;}
            int savedResWidth = SettingsManager.Instance.settings.resWidth;
            int savedResHeight = SettingsManager.Instance.settings.resHeight;
            Dictionary<Tuple<int, int>, int> selectedResolutionDictionary = new Dictionary<Tuple<int, int>, int>() {
                {new Tuple<int, int>(3840, 2400), 0},
                {new Tuple<int, int>(3840, 2160), 1},
                {new Tuple<int, int>(2560, 1600), 2},
                {new Tuple<int, int>(2560, 1440), 3},
                {new Tuple<int, int>(1920, 1200), 4},
                {new Tuple<int, int>(1920, 1080), 5},
                {new Tuple<int, int>(1280, 800), 6},
                {new Tuple<int, int>(1280, 720), 7},
            };
            resolutionSelect.value = selectedResolutionDictionary[new Tuple<int, int>(savedResWidth, savedResHeight)];
            fullscreenCheckbox.isOn = SettingsManager.Instance.settings.fullscreen;
            vsyncToggle.isOn = SettingsManager.Instance.settings.vsync;
            updateScreenSettings(false);
        }

        private void onVolumeValueChange(float value, string tag)
        {
            switch (tag)
            {
                case "main":
                    mainVolumeValueLabel.text = ((int)(value * 100f)).ToString();
                    break;
                case "music":
                    musicVolumeValueLabel.text = ((int)(value * 100f)).ToString();
                    break;
            }
        }

        public void updateScreenSettings(bool save)
        {
            bool fullscreen = fullscreenCheckbox.isOn;
            Debug.Log(vsyncToggle.isOn);
            bool vsync = vsyncToggle.isOn;
            updateVsync(vsync);
            Dictionary<int, int[]> resolutionDictionary = new Dictionary<int, int[]>() {
                {0, new int[] {3840, 2400}},
                {1, new int[] {3840, 2160}},
                {2, new int[] {2560, 1600}},
                {3, new int[] {2560, 1440}},
                {4, new int[] {1920, 1200}},
                {5, new int[] {1920, 1080}},
                {6, new int[] {1280, 800}},
                {7, new int[] {1280, 720}}
            };

            int[] newResolution = resolutionDictionary[resolutionSelect.value];
            Screen.SetResolution(newResolution[0], newResolution[1], fullscreen);
            SettingsManager.Instance.updateScreenSettings(newResolution[0], newResolution[1], fullscreen, vsync, save);
        }
        private void updateVsync(bool status)
        {
            if (status)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }
        public void resumeGame()
        {
            GameManager.Instance.resume();
        }

        public void showSettingsMenu()
        {
            resumeButton.SetActive(false);
            settingsButton.SetActive(false);
            quitButton.SetActive(false);
            exitButton.SetActive(false);
            settingsContainer.SetActive(true);
        }

        public void hideSettingsMenu()
        {
            resumeButton.SetActive(true);
            settingsButton.SetActive(true);
            quitButton.SetActive(true);
            exitButton.SetActive(true);
            settingsContainer.SetActive(false);
        }

        public void quitMatch()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void exit()
        {
            Application.Quit();
        }
    }
}
