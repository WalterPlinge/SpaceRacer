using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        //SceneManager.LoadSceneAsync("Start Button", LoadSceneMode.Additive);
        //SceneManager.LoadSceneAsync("Menu Background", LoadSceneMode.Additive);
    }


    // Update is called once per frame
    public void Update()
    {
        
    }
		

	// Loads the main menu scene
	public void LoadMenuScene()
	{
		SceneManager.LoadSceneAsync("Menu Buttons", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("Start Button");
	}


	// Loads the game scene
	public void LoadGameScene()
	{
		SceneManager.LoadSceneAsync("Space Loop");
        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("Start Button");
	}


	// Quits the application
	public void QuitButton()
	{
		Application.Quit();
	}
}
