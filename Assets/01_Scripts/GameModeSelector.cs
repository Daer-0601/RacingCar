using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject gameModeMenu;
    public GameObject mainMenu;

    void Start()
    {
        // Asegurar que el GameModeManager existe
        if (GameModeManager.Instance == null)
        {
            GameObject managerObj = new GameObject("GameModeManager");
            managerObj.AddComponent<GameModeManager>();
        }
    }

    public void SelectSoloVsBot()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.SoloVsBot);
        }
        LoadGame();
    }

    public void SelectOneVsOne()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.OneVsOne);
        }
        LoadGame();
    }

    public void BackToMainMenu()
    {
        if (gameModeMenu != null)
            gameModeMenu.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Level_01");
    }
}

