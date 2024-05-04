using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas settingCanvas;
    [SerializeField] private Canvas skirmishCanvas;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Animator menuAnimator;
    [SerializeField] private Image[] menuBackgrounds;
    [SerializeField] private Sprite[] menuBackgroundImages;
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

    public void startGame()
    {

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
        List<int> backgroundIndexes = new List<int>();

        while (backgroundIndexes.Count < menuBackgrounds.Length)
        {
            int newIndex = Random.Range(0, 9);
            if (!backgroundIndexes.Contains(newIndex))
                backgroundIndexes.Add(newIndex);
        }

        int i = 0;
        foreach (Image backgroundImage in menuBackgrounds)
        {
            Debug.Log(backgroundIndexes[i]);
            backgroundImage.sprite = menuBackgroundImages[backgroundIndexes[i]];
            i++;
        }
    }
}
