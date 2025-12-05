using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject optionMenu;
    public GameObject mainMenu;
    public GameObject playerSelectMenu; // Panel para elegir número de jugadores
    public GameObject levelSelectMenu;  // Panel para elegir nivel
    
    [Header("Controlador de Pantalla de Controles")]
    public ControlsDisplayController controlsDisplay;

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
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (optionMenu != null)
            optionMenu.SetActive(false);
        
        // Buscar ControlsDisplayController si no está asignado
        if (controlsDisplay == null)
        {
            controlsDisplay = FindObjectOfType<ControlsDisplayController>();
        }
    }

    public void OpenOptions()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (optionMenu != null)
            optionMenu.SetActive(true);
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
    }

    public void OpenMainMenuPanel()
    {
        if (mainMenu != null)
            mainMenu.SetActive(true);
        if (optionMenu != null)
            optionMenu.SetActive(false);
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
    }

    // Cuando presiona el botón "Jugar" - muestra opciones de jugadores
    public void PlayGame()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(true);
        if (optionMenu != null)
            optionMenu.SetActive(false);
    }

    // ========== SELECCIÓN DE NÚMERO DE JUGADORES ==========

    // Seleccionar 2 jugadores
    public void Select2Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.TwoPlayers);
        }
        OpenLevelSelectMenu();
    }

    // Seleccionar 3 jugadores
    public void Select3Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.ThreePlayers);
        }
        OpenLevelSelectMenu();
    }

    // Seleccionar 4 jugadores
    public void Select4Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.FourPlayers);
        }
        OpenLevelSelectMenu();
    }

    // Mostrar menú de selección de nivel
    public void OpenLevelSelectMenu()
    {
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(true);
    }

    // ========== SELECCIÓN DE NIVEL ==========

    // Seleccionar Nivel 1
    public void SelectLevel1()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(1);
        }
        ShowControlsScreen();
    }

    // Seleccionar Nivel 2
    public void SelectLevel2()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(2);
        }
        ShowControlsScreen();
    }

    // Seleccionar Nivel 3
    public void SelectLevel3()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetSelectedLevel(3);
        }
        ShowControlsScreen();
    }
    
    // Mostrar pantalla de controles
    private void ShowControlsScreen()
    {
        int playerCount = 2;
        int selectedLevel = 1;
        
        if (GameModeManager.Instance != null)
        {
            playerCount = GameModeManager.Instance.PlayerCount;
            selectedLevel = GameModeManager.Instance.SelectedLevel;
        }
        
        // Mostrar pantalla de controles
        if (controlsDisplay != null)
        {
            controlsDisplay.ShowControls(playerCount, selectedLevel);
        }
        else
        {
            // Si no hay ControlsDisplayController, cargar directamente (fallback)
            Debug.LogWarning("ControlsDisplayController no encontrado. Cargando escena directamente.");
            LoadGameScene();
        }
    }

    // ========== NAVEGACIÓN ==========

    // Volver al menú principal desde la selección de jugadores
    public void BackToMainMenu()
    {
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    // Volver a la selección de jugadores desde la selección de nivel
    public void BackToPlayerSelectMenu()
    {
        if (levelSelectMenu != null)
            levelSelectMenu.SetActive(false);
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(true);
        if (controlsDisplay != null)
        {
            controlsDisplay.HideControls();
        }
    }
    
    // Volver desde la pantalla de controles a la selección de nivel
    public void BackToLevelSelectFromControls()
    {
        if (controlsDisplay != null)
        {
            controlsDisplay.HideControls();
        }
        if (levelSelectMenu != null)
        {
            levelSelectMenu.SetActive(true);
        }
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
