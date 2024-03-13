using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Canvas mainCanvas;
    public Canvas settingCanvas;
    public Canvas skirmishCanvas;
    public Camera menuCamera;

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
