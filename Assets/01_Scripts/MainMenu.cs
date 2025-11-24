using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject optionMenu;
    public GameObject mainMenu;

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
		mainMenu.SetActive(true);
		optionMenu.SetActive(false);
	}

	public void QuitGame()
	{
        Application.Quit();
	}

    public void PlayGame()
    {
        SceneManager.LoadScene("Level_01");
    }
}
