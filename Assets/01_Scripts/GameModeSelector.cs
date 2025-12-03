using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelector : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject playerSelectMenu;
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

    public void Select2Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.TwoPlayers);
        }
        LoadGame();
    }

    public void Select3Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.ThreePlayers);
        }
        LoadGame();
    }

    public void Select4Players()
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.FourPlayers);
        }
        LoadGame();
    }

    public void BackToMainMenu()
    {
        if (playerSelectMenu != null)
            playerSelectMenu.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    private void LoadGame()
    {
        string sceneName = "Level_01";
        
        if (GameModeManager.Instance != null)
        {
            sceneName = GameModeManager.Instance.GetLevelSceneName();
        }
        
        SceneManager.LoadScene(sceneName);
    }
}
