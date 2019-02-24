using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Close escape menu
    public void ResumeButton()
    {
        SceneManager.UnloadSceneAsync("EscapeMenu");
    }


    // Resets the game
    public void RestartButton()
    {
        SceneManager.UnloadSceneAsync("EscapeMenu");

    }


    // Quits the application
    public void QuitButton()
    {
        Application.Quit();
    }
}
