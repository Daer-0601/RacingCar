using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject optionMenu;
    public GameObject mainMenu;
    public GameObject gameModeMenu; // Panel para elegir modo de juego
    public GameObject levelSelectMenu; // Panel para elegir nivel

    void Start()
    {
        // Asegurar que el GameModeManager existe
        if (GameModeManager.Instance == null)
        {
            GameObject managerObj = new GameObject("GameModeManager");
            managerObj.AddComponent<GameModeManager>();
        }

        // Asegurar que el menú principal está visible al inicio
        if (mainMenu != null)
            mainMenu.SetActive(true);
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (optionMenu != null)
            optionMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (optionMenu != null)
            optionMenu.SetActive(true);
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
    }

    public void OpenMainMenuPanel()
    {
        if (mainMenu != null)
            mainMenu.SetActive(true);
        if (optionMenu != null)
            optionMenu.SetActive(false);
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
    }

    // Cuando presiona el botón "Jugar" - muestra opciones de modo
    public void PlayGame()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (gameModeMenu != null)
            gameModeMenu.SetActive(true);
        if (optionMenu != null)
            optionMenu.SetActive(false);
    }

    // Seleccionar modo: Solo vs Bot
    public void SelectSoloVsBot()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.SoloVsBot);
        }
        OpenLevelSelectMenu();
    }

    // Seleccionar modo: 1 vs 1
    public void SelectOneVsOne()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.OneVsOne);
        }
        OpenLevelSelectMenu();
    }

    // Mostrar menú de selección de nivel
    public void OpenLevelSelectMenu()
    {
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(true);
    }

    // Seleccionar Nivel 1
    public void SelectLevel1()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(1);
        }
        LoadGameScene();
    }

    // Seleccionar Nivel 2
    public void SelectLevel2()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(2);
        }
        LoadGameScene();
    }

    // Seleccionar Nivel 3
    public void SelectLevel3()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(3);
        }
        LoadGameScene();
    }

    // Volver al menú principal desde la selección de modo
    public void BackToMainMenu()
    {
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    // Volver a la selección de modo desde la selección de nivel
    public void BackToGameModeMenu()
    {
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (gameModeMenu != null)
            gameModeMenu.SetActive(true);
    }

    // Cargar la escena del juego según el nivel seleccionado
    private void LoadGameScene()
    {
        string sceneName = "Level_01"; // Por defecto
        
        if (GameModeManager.Instance != null)
        {
            sceneName = GameModeManager.Instance.GetLevelSceneName();
        }
        
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Detener el modo Play en el Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Cerrar la aplicación en una build real
        Application.Quit();
#endif
    }
}
