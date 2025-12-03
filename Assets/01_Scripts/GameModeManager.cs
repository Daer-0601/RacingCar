using UnityEngine;

public enum GameMode
{
    SoloVsBot,      // Un jugador con Arduino vs Bot
    OneVsOne        // Un jugador con Arduino vs Un jugador con teclado
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; } = GameMode.SoloVsBot;
    public int SelectedLevel { get; private set; } = 1; // Nivel seleccionado (1, 2 o 3)

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGameMode(GameMode mode)
    {
        CurrentGameMode = mode;
        Debug.Log("Modo de juego seleccionado: " + mode.ToString());
    }

    public void SetSelectedLevel(int level)
    {
        SelectedLevel = Mathf.Clamp(level, 1, 3);
        Debug.Log("Nivel seleccionado: " + SelectedLevel);
    }

    public string GetLevelSceneName()
    {
        return "Level_0" + SelectedLevel;
    }
}

