using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour
{

    // Start is called before the first frame update
    public void Start()
    {
        //Loads start button scene if no menu scenes are present
        if (SceneManager.GetSceneByName("Start Button").isLoaded == false && SceneManager.GetSceneByName("Menu Buttons").isLoaded == false)
        {
            SceneManager.LoadSceneAsync("Start Button", LoadSceneMode.Additive);
        }
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


	// Loads the race track
	public void LoadGameScene()
	{
		SceneManager.LoadSceneAsync("Basic Track");
        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("Start Button");
	}


    // Loads controls menu
    public void LoadControlsScene()
    {
        SceneManager.UnloadSceneAsync("Start Button");
        SceneManager.LoadSceneAsync("Control Menu", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("KeyboardImage", LoadSceneMode.Additive);
       
    }


    // Display keyboard controls
    public void DisplayKeyboard()
    {
        UnloadControls();

        SceneManager.LoadSceneAsync("KeyboardImage");
    }


    // Display playstation controls
    public void DisplayPS4()
    {
        UnloadControls();

        SceneManager.LoadSceneAsync("PS4Image");
    }


    // Display XBOX controls
    public void DisplayXBOX()
    {
        UnloadControls();

        SceneManager.LoadSceneAsync("XBOXImage");
    }


    // Quits the application
    public void QuitButton()
	{
		Application.Quit();
	}


    // Removes currently displayed control scheme
    public void UnloadControls()
    {
        if (SceneManager.GetSceneByName("KeyboardImage").isLoaded == true)
        {
            SceneManager.UnloadSceneAsync("KeyboardImage");
        }
        else if (SceneManager.GetSceneByName("PS4Image").isLoaded == true)
        {
            SceneManager.UnloadSceneAsync("PS4Image");
        }
        else if (SceneManager.GetSceneByName("XBOXImage").isLoaded == true)
        {
            SceneManager.UnloadSceneAsync("XBOXImage");
        }
    }


    // Goes back to main menu from controls page
    public void ControlsBack()
    {
        SceneManager.LoadSceneAsync("Menu Background");
        SceneManager.LoadSceneAsync("Menu Buttons", LoadSceneMode.Additive);
    }
}
