using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [Header("Referencias de Jugadores")]
    public GameObject player1; // Jugador 1 (Arduino)
    public GameObject player2; // Jugador 2 (Flechas ↑↓←→)
    public GameObject player3; // Jugador 3 (IJKL)
    public GameObject player4; // Jugador 4 (IJKL)

    [Header("Componentes de Control")]
    public ArduinoSteering arduinoSteering;
    public CarMovement carMovementP1;
    public CarPlayer2 carPlayer2;
    public CarPlayer3 carPlayer3;
    public CarPlayer4 carPlayer4;

    [Header("Cámaras")]
    public SplitScreenManager splitScreenManager;
    public Camera mainCamera;

    [Header("Countdown")]
    public CountdownController countdownController;

    private int playerCount = 2;

    void Start()
    {
        FindComponentsIfNeeded();
        DisableAllBots();
        ConfigureGameMode();
        InitializeCountdown();
    }

    // Desactivar todos los bots - ya no se usan
    private void DisableAllBots()
    {
        CarBot[] allBots = FindObjectsOfType<CarBot>();
        foreach (CarBot bot in allBots)
        {
            bot.enabled = false;
            Debug.Log("Bot desactivado en: " + bot.gameObject.name);
        }
    }

    private void FindComponentsIfNeeded()
    {
        // Buscar todos los jugadores por tag
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        
        // Buscar Player1 si no está asignado (el que tiene Arduino)
        if (player1 == null)
        {
            foreach (GameObject p in allPlayers)
            {
                CarMovement cm = p.GetComponent<CarMovement>();
                if (cm != null)
                {
                    player1 = p;
                    Debug.Log("Player1 encontrado: " + p.name);
                    break;
                }
            }
        }

        // Buscar CarMovement en Player1
        if (carMovementP1 == null && player1 != null)
        {
            carMovementP1 = player1.GetComponent<CarMovement>();
        }

        // Buscar ArduinoSteering en Player1
        if (arduinoSteering == null && player1 != null)
        {
            arduinoSteering = player1.GetComponent<ArduinoSteering>();
            if (arduinoSteering == null)
            {
                arduinoSteering = FindObjectOfType<ArduinoSteering>();
            }
        }

        // Buscar Player2 si no está asignado
        if (player2 == null)
        {
            foreach (GameObject p in allPlayers)
            {
                if (p != player1)
                {
                    player2 = p;
                    Debug.Log("Player2 encontrado: " + p.name);
                    break;
                }
            }
            
            if (player2 == null)
            {
                player2 = GameObject.Find("Car2") ?? GameObject.Find("Player2");
            }
        }

        // Buscar/crear CarPlayer2 en Player2
        if (carPlayer2 == null && player2 != null)
        {
            carPlayer2 = player2.GetComponent<CarPlayer2>();
        }

        // Buscar Player3 si no está asignado
        if (player3 == null)
        {
            player3 = GameObject.Find("Car3") ?? GameObject.Find("Player3");
        }

        // Buscar CarPlayer3 en Player3
        if (carPlayer3 == null && player3 != null)
        {
            carPlayer3 = player3.GetComponent<CarPlayer3>();
        }

        // Buscar Player4 si no está asignado
        if (player4 == null)
        {
            player4 = GameObject.Find("Car4") ?? GameObject.Find("Player4");
        }

        // Buscar CarPlayer4 en Player4
        if (carPlayer4 == null && player4 != null)
        {
            carPlayer4 = player4.GetComponent<CarPlayer4>();
        }

        // Buscar SplitScreenManager si no está asignado
        if (splitScreenManager == null)
        {
            splitScreenManager = FindObjectOfType<SplitScreenManager>();
        }
        
        // Buscar CountdownController si no está asignado
        if (countdownController == null)
        {
            countdownController = FindObjectOfType<CountdownController>();
            if (countdownController == null)
            {
                // Crear CountdownController si no existe
                GameObject countdownObj = new GameObject("CountdownController");
                countdownController = countdownObj.AddComponent<CountdownController>();
            }
        }
    }

    private void ConfigureGameMode()
    {
        if (GameModeManager.Instance == null)
        {
            Debug.LogWarning("GameModeManager no encontrado. Usando modo por defecto: 2 Jugadores");
            playerCount = 2;
        }
        else
        {
            playerCount = GameModeManager.Instance.PlayerCount;
        }

        Debug.Log("=== CONFIGURANDO JUEGO PARA " + playerCount + " JUGADORES ===");
        LogControlsInfo();

        // Configurar cada jugador según el modo
        ConfigurePlayer1();
        ConfigurePlayer2();
        ConfigurePlayer3();
        ConfigurePlayer4();
        ConfigureCameras();
    }

    private void LogControlsInfo()
    {
        Debug.Log("CONTROLES:");
        Debug.Log("  P1: Arduino (volante + pedales)");
        Debug.Log("  P2: Flechas (↑ acelerar, ↓ frenar, ← → girar)");
        if (playerCount >= 3)
            Debug.Log("  P3: Teclas WASD (W acelerar, S frenar, A D girar)");
        if (playerCount >= 4)
            Debug.Log("  P4: Teclas IJKL (I acelerar, K frenar, J L girar)");
    }

    private void ConfigurePlayer1()
    {
        // Player 1 siempre activo - usa Arduino
        if (player1 != null)
        {
            player1.SetActive(true);
            
            if (arduinoSteering != null)
            {
                arduinoSteering.enabled = true;
                Debug.Log("✓ Player 1: Arduino ACTIVADO");
            }
            else
            {
                Debug.LogWarning("⚠ Arduino no encontrado para Player 1");
            }

            if (carMovementP1 != null)
            {
                carMovementP1.enabled = true;
                carMovementP1.useKeyboardInput = false; // P1 solo usa Arduino
                Debug.Log("✓ Player 1: CarMovement configurado (Arduino)");
            }
        }
        else
        {
            Debug.LogError("✗ Player 1 NO ENCONTRADO");
        }
    }

    private void ConfigurePlayer2()
    {
        // Player 2 siempre activo - usa flechas del teclado
        if (player2 != null)
        {
            player2.SetActive(true);

            if (carPlayer2 == null)
            {
                carPlayer2 = player2.GetComponent<CarPlayer2>();
                if (carPlayer2 == null)
                {
                    carPlayer2 = player2.AddComponent<CarPlayer2>();
                }
            }
            
            carPlayer2.enabled = true;
            
            // Configurar teclas de flechas
            carPlayer2.accelerateKey = KeyCode.UpArrow;
            carPlayer2.brakeKey = KeyCode.DownArrow;
            carPlayer2.leftKey = KeyCode.LeftArrow;
            carPlayer2.rightKey = KeyCode.RightArrow;
            
            Debug.Log("✓ Player 2: Flechas (↑↓←→)");
        }
        else
        {
            Debug.LogError("✗ Player 2 NO ENCONTRADO");
        }
    }

    private void ConfigurePlayer3()
    {
        if (player3 != null)
        {
            bool shouldBeActive = playerCount >= 3;
            player3.SetActive(shouldBeActive);

            if (shouldBeActive)
            {
                // IMPORTANTE: Desactivar CarPlayer2 si existe en este objeto
                CarPlayer2 wrongComponent = player3.GetComponent<CarPlayer2>();
                if (wrongComponent != null)
                {
                    wrongComponent.enabled = false;
                    Debug.Log("⚠ CarPlayer2 desactivado en Player3 (usará CarPlayer3)");
                }

                // Desactivar CarBot si existe
                CarBot botComponent = player3.GetComponent<CarBot>();
                if (botComponent != null)
                {
                    botComponent.enabled = false;
                }

                if (carPlayer3 == null)
                {
                    carPlayer3 = player3.GetComponent<CarPlayer3>();
                    if (carPlayer3 == null)
                    {
                        carPlayer3 = player3.AddComponent<CarPlayer3>();
                    }
                }
                
                carPlayer3.enabled = true;
                
                // Configurar teclas WASD
                carPlayer3.accelerateKey = KeyCode.W;
                carPlayer3.brakeKey = KeyCode.S;
                carPlayer3.leftKey = KeyCode.A;
                carPlayer3.rightKey = KeyCode.D;
                
                Debug.Log("✓ Player 3: Teclas WASD (W acelerar, S frenar, A D girar)");
            }
            else
            {
                player3.SetActive(false);
            }
        }
        else if (playerCount >= 3)
        {
            Debug.LogError("✗ Player 3 NO ENCONTRADO - Necesario para " + playerCount + " jugadores");
        }
    }

    private void ConfigurePlayer4()
    {
        if (player4 != null)
        {
            bool shouldBeActive = playerCount >= 4;
            player4.SetActive(shouldBeActive);

            if (shouldBeActive)
            {
                // IMPORTANTE: Desactivar otros componentes de control si existen
                CarPlayer2 cp2 = player4.GetComponent<CarPlayer2>();
                if (cp2 != null) cp2.enabled = false;
                
                CarPlayer3 cp3 = player4.GetComponent<CarPlayer3>();
                if (cp3 != null) cp3.enabled = false;
                
                CarBot botComponent = player4.GetComponent<CarBot>();
                if (botComponent != null) botComponent.enabled = false;

                if (carPlayer4 == null)
                {
                    carPlayer4 = player4.GetComponent<CarPlayer4>();
                    if (carPlayer4 == null)
                    {
                        carPlayer4 = player4.AddComponent<CarPlayer4>();
                    }
                }
                
                carPlayer4.enabled = true;
                
                // Configurar teclas IJKL
                carPlayer4.accelerateKey = KeyCode.I;
                carPlayer4.brakeKey = KeyCode.K;
                carPlayer4.leftKey = KeyCode.J;
                carPlayer4.rightKey = KeyCode.L;
                
                Debug.Log("✓ Player 4: Teclas IJKL (I acelerar, K frenar, J L girar)");
            }
            else
            {
                player4.SetActive(false);
            }
        }
        else if (playerCount >= 4)
        {
            Debug.LogError("✗ Player 4 NO ENCONTRADO - Necesario para 4 jugadores");
        }
    }

    private void ConfigureCameras()
    {
        if (splitScreenManager != null)
        {
            splitScreenManager.enabled = true;
            splitScreenManager.SetPlayerCount(playerCount);

            // Asignar referencias de jugadores al SplitScreenManager
            if (splitScreenManager.player1 == null && player1 != null)
                splitScreenManager.player1 = player1.transform;
            if (splitScreenManager.player2 == null && player2 != null)
                splitScreenManager.player2 = player2.transform;
            if (splitScreenManager.player3 == null && player3 != null)
                splitScreenManager.player3 = player3.transform;
            if (splitScreenManager.player4 == null && player4 != null)
                splitScreenManager.player4 = player4.transform;

            Debug.Log("✓ Split Screen: " + playerCount + " pantallas");
        }

        // Desactivar cámara principal
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }
    
    private void InitializeCountdown()
    {
        // Configurar CountdownController
        if (countdownController != null)
        {
            countdownController.gameModeController = this;
            // El countdown se iniciará automáticamente en su Start()
        }
    }
}
