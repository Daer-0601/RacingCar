using UnityEngine;

public enum GameMode
{
    TwoPlayers,     // 2 jugadores
    ThreePlayers,   // 3 jugadores
    FourPlayers     // 4 jugadores
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; } = GameMode.TwoPlayers;
    public int PlayerCount { get; private set; } = 2;
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
        
        // Actualizar el número de jugadores según el modo
        switch (mode)
        {
            case GameMode.TwoPlayers:
                PlayerCount = 2;
                break;
            case GameMode.ThreePlayers:
                PlayerCount = 3;
                break;
            case GameMode.FourPlayers:
                PlayerCount = 4;
                break;
        }
        
        Debug.Log("Modo de juego seleccionado: " + mode.ToString() + " (" + PlayerCount + " jugadores)");
    }

    public void SetPlayerCount(int count)
    {
        PlayerCount = Mathf.Clamp(count, 2, 4);
        
        // Actualizar el modo según el número de jugadores
        switch (PlayerCount)
        {
            case 2:
                CurrentGameMode = GameMode.TwoPlayers;
                break;
            case 3:
                CurrentGameMode = GameMode.ThreePlayers;
                break;
            case 4:
                CurrentGameMode = GameMode.FourPlayers;
                break;
        }
        
        Debug.Log("Número de jugadores: " + PlayerCount);
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
