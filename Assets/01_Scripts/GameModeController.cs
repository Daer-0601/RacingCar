using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [Header("Referencias de Jugadores")]
    public GameObject player1; // Jugador con Arduino
    public GameObject player2; // Jugador 2 o Bot

    [Header("Componentes de Control")]
    public ArduinoSteering arduinoSteering;
    public CarMovement carMovementP1;
    public CarPlayer2 carPlayer2;
    public CarBot carBot;

    [Header("Cámaras")]
    public SplitScreenManager splitScreenManager;
    public Camera mainCamera; // Cámara única para modo solo

    void Start()
    {
        // Buscar componentes automáticamente si no están asignados
        FindComponentsIfNeeded();
        ConfigureGameMode();
    }

    private void FindComponentsIfNeeded()
    {
        // Buscar Player1 si no está asignado
        if (player1 == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player1 = foundPlayer;
                Debug.Log("Player1 encontrado automáticamente: " + foundPlayer.name);
            }
        }

        // Buscar CarMovement en Player1
        if (carMovementP1 == null && player1 != null)
        {
            carMovementP1 = player1.GetComponent<CarMovement>();
            if (carMovementP1 != null)
                Debug.Log("CarMovement encontrado en Player1");
        }

        // Buscar ArduinoSteering en Player1
        if (arduinoSteering == null && player1 != null)
        {
            arduinoSteering = player1.GetComponent<ArduinoSteering>();
            if (arduinoSteering == null)
            {
                // Buscar en toda la escena
                arduinoSteering = FindObjectOfType<ArduinoSteering>();
            }
            if (arduinoSteering != null)
                Debug.Log("ArduinoSteering encontrado");
        }

        // Buscar Player2 si no está asignado (buscar por nombre común)
        if (player2 == null)
        {
            GameObject[] allCars = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject car in allCars)
            {
                if (car != player1)
                {
                    player2 = car;
                    Debug.Log("Player2 encontrado automáticamente: " + car.name);
                    break;
                }
            }

            // Si no se encuentra por tag, buscar por nombre
            if (player2 == null)
            {
                GameObject foundP2 = GameObject.Find("Car2") ?? GameObject.Find("Player2");
                if (foundP2 != null)
                    player2 = foundP2;
            }
        }

        // Buscar CarPlayer2 en Player2
        if (carPlayer2 == null && player2 != null)
        {
            carPlayer2 = player2.GetComponent<CarPlayer2>();
            if (carPlayer2 != null)
                Debug.Log("CarPlayer2 encontrado en Player2");
        }

        // Buscar CarBot en Player2
        if (carBot == null && player2 != null)
        {
            carBot = player2.GetComponent<CarBot>();
            if (carBot != null)
                Debug.Log("CarBot encontrado en Player2");
        }

        // Buscar SplitScreenManager si no está asignado
        if (splitScreenManager == null)
        {
            splitScreenManager = FindObjectOfType<SplitScreenManager>();
            if (splitScreenManager != null)
                Debug.Log("SplitScreenManager encontrado automáticamente");
        }
    }

    private void ConfigureGameMode()
    {
        if (GameModeManager.Instance == null)
        {
            Debug.LogWarning("GameModeManager no encontrado. Usando modo por defecto: SoloVsBot");
            ConfigureSoloVsBot();
            return;
        }

        GameMode mode = GameModeManager.Instance.CurrentGameMode;

        switch (mode)
        {
            case GameMode.SoloVsBot:
                ConfigureSoloVsBot();
                break;
            case GameMode.OneVsOne:
                ConfigureOneVsOne();
                break;
            default:
                ConfigureSoloVsBot();
                break;
        }
    }

    private void ConfigureSoloVsBot()
    {
        Debug.Log("Configurando modo: Solo vs Bot");

        // ========== CONFIGURAR PLAYER 1 (Arduino) ==========
        // Activar Arduino para P1
        if (arduinoSteering != null)
        {
            arduinoSteering.enabled = true;
            Debug.Log("✓ Arduino activado para Player 1");
        }
        else
        {
            Debug.LogWarning("⚠ ArduinoSteering no encontrado para Player 1");
        }

        // Configurar CarMovement para P1
        if (carMovementP1 != null)
        {
            carMovementP1.useKeyboardInput = false; // Desactivar teclado para P1
            carMovementP1.enabled = true;
            Debug.Log("✓ CarMovement configurado para Player 1 (sin teclado)");
        }
        else
        {
            Debug.LogWarning("⚠ CarMovement no encontrado para Player 1");
        }

        // ========== CONFIGURAR PLAYER 2 (Bot) ==========
        // Desactivar control de teclado para P2
        if (carPlayer2 != null)
        {
            carPlayer2.enabled = false;
            Debug.Log("✓ CarPlayer2 desactivado para Player 2");
        }

        // Activar Bot para P2
        if (carBot != null)
        {
            carBot.enabled = true;
            
            // Asegurar que el bot tenga el target correcto (Player1)
            if (carBot.target == null && player1 != null)
            {
                carBot.target = player1.transform;
                Debug.Log("✓ Target del Bot asignado a Player 1");
            }
            
            Debug.Log("✓ CarBot activado para Player 2");
        }
        else
        {
            Debug.LogWarning("⚠ CarBot no encontrado para Player 2. ¿Agregaste el componente CarBot al Player 2?");
        }

        // ========== CONFIGURAR CÁMARAS ==========
        // Configurar split screen
        if (splitScreenManager != null)
        {
            splitScreenManager.enabled = true;
            
            // Asegurar referencias en SplitScreenManager
            if (splitScreenManager.player1 == null && player1 != null)
                splitScreenManager.player1 = player1.transform;
            if (splitScreenManager.player2 == null && player2 != null)
                splitScreenManager.player2 = player2.transform;
                
            Debug.Log("✓ SplitScreen activado");
        }

        // Desactivar cámara principal si existe
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            Debug.Log("✓ Cámara principal desactivada");
        }
    }

    private void ConfigureOneVsOne()
    {
        Debug.Log("Configurando modo: 1 vs 1");

        // ========== CONFIGURAR PLAYER 1 (Arduino) ==========
        // Activar Arduino para P1
        if (arduinoSteering != null)
        {
            arduinoSteering.enabled = true;
            Debug.Log("✓ Arduino activado para Player 1");
        }
        else
        {
            Debug.LogWarning("⚠ ArduinoSteering no encontrado para Player 1");
        }

        // Configurar CarMovement para P1
        if (carMovementP1 != null)
        {
            carMovementP1.useKeyboardInput = false; // Desactivar teclado para P1
            carMovementP1.enabled = true;
            Debug.Log("✓ CarMovement configurado para Player 1 (sin teclado)");
        }
        else
        {
            Debug.LogWarning("⚠ CarMovement no encontrado para Player 1");
        }

        // ========== CONFIGURAR PLAYER 2 (Teclado) ==========
        // Desactivar bot
        if (carBot != null)
        {
            carBot.enabled = false;
            Debug.Log("✓ CarBot desactivado para Player 2");
        }

        // Activar control de teclado para P2
        if (carPlayer2 != null)
        {
            carPlayer2.enabled = true;
            Debug.Log("✓ CarPlayer2 activado para Player 2 (teclado)");
        }
        else
        {
            Debug.LogWarning("⚠ CarPlayer2 no encontrado para Player 2. ¿Agregaste el componente CarPlayer2 al Player 2?");
        }

        // ========== CONFIGURAR CÁMARAS ==========
        // Configurar split screen
        if (splitScreenManager != null)
        {
            splitScreenManager.enabled = true;
            
            // Asegurar referencias en SplitScreenManager
            if (splitScreenManager.player1 == null && player1 != null)
                splitScreenManager.player1 = player1.transform;
            if (splitScreenManager.player2 == null && player2 != null)
                splitScreenManager.player2 = player2.transform;
                
            Debug.Log("✓ SplitScreen activado");
        }

        // Desactivar cámara principal si existe
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            Debug.Log("✓ Cámara principal desactivada");
        }
    }
}

