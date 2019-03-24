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
		Time.timeScale = 1;
	}


	// Resets the game
	public void RestartButton()
	{
		SceneManager.LoadSceneAsync("Space Loop");
		SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("EscapeMenu");
		Time.timeScale = 1;
	}


	// Quits the application
	public void QuitButton()
	{
		Application.Quit();
	}
}
