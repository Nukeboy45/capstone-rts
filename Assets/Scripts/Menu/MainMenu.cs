using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas settingCanvas;
    [SerializeField] private Canvas skirmishCanvas;
    [SerializeField] private Camera menuCamera;

    public void settingMenu()
    {
        menuCamera.GetComponent<Animator>().SetInteger("camPos", 1);
    }

    public void skirmishMenu()
    {
        menuCamera.GetComponent<Animator>().SetInteger("camPos", 1);
    }

    public void mainMenu()
    {
        menuCamera.GetComponent<Animator>().SetInteger("camPos", 0);
    }
    
    public void exitGame()
    {
        Application.Quit();
    }

    public void startGame()
    {

    }
}
