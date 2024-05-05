using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Camera menuCamera;
    [SerializeField] private Animator menuAnimator;
        
    [Header("Main Menu Screen")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Image mainScreenBackground;
    [SerializeField] private Sprite[] mainScreenBackgroundImages;

    [Header("Skirmish Menu Screen")]
    [SerializeField] private Canvas skirmishCanvas;
    [SerializeField] private Image skirmishScreenBackground;
    [SerializeField] private Sprite[] skirmishScreenBackgroundImages;

    [Header("Settings Menu Screen")]
    [SerializeField] private Canvas settingCanvas;
    [SerializeField] private Image settingScreenBackground;
    [SerializeField] private Sprite[] settingScreenBackgroundImages;
    [SerializeField] private List<Canvas> menus = new List<Canvas>();


    private void Start()
    {
        getRandomBackgrounds();
    }
    public void Update()
    {
        updateAspectRatios();
    }

    public void settingMenu()
    {
        menuAnimator.SetInteger("menuState", 2);
    }

    public void skirmishMenu()
    {
        menuAnimator.SetInteger("menuState", 1);
    }

    public void mainMenu()
    {
        menuAnimator.SetInteger("menuState", 0);
    }
    
    public void exitGame()
    {
        Application.Quit();
    }

    public void updateAspectRatios()
    {
        foreach (Canvas canvas in menus)
        {
            RectTransform rt = canvas.GetComponent<RectTransform>();
            float canvasHeight = rt.rect.height;
            float desiredWidth = canvasHeight * menuCamera.aspect;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
        }
    }

    private void getRandomBackgrounds()
    {
        mainScreenBackground.sprite = mainScreenBackgroundImages[Random.Range(0,3)];
        skirmishScreenBackground.sprite = skirmishScreenBackgroundImages[Random.Range(0,3)];
        settingScreenBackground.sprite = settingScreenBackgroundImages[Random.Range(0,3)];
    }
}
