using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CountdownController : MonoBehaviour
{
    [Header("UI Countdown")]
    public TextMeshProUGUI countdownText;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countdownSound; // Sonido que dura 4 segundos
    
    [Header("Referencias de Jugadores")]
    public GameModeController gameModeController;
    
    [Header("Configuración")]
    public float countdownInterval = 1f; // Tiempo entre cada número (3, 2, 1)
    public float startDisplayTime = 0.5f; // Tiempo que se muestra "START"
    
    private bool countdownActive = false;
    private bool raceStarted = false;
    
    void Start()
    {
        // Buscar GameModeController si no está asignado
        if (gameModeController == null)
        {
            gameModeController = FindObjectOfType<GameModeController>();
        }
        
        // Crear UI de countdown si no existe
        if (countdownText == null)
        {
            CreateCountdownUI();
        }
        
        // Crear AudioSource si no existe
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;
        }
        
        // Iniciar countdown después de un pequeño delay para asegurar que todo esté configurado
        StartCoroutine(DelayedStartCountdown());
    }
    
    IEnumerator DelayedStartCountdown()
    {
        // Esperar un frame para asegurar que GameModeController haya terminado de configurar todo
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        // Iniciar countdown
        StartCountdown();
    }
    
    void CreateCountdownUI()
    {
        // Buscar Canvas existente o crear uno nuevo
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("CountdownCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Asegurar que esté por encima de todo
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Crear GameObject para el texto
        GameObject textObj = new GameObject("CountdownText");
        textObj.transform.SetParent(canvas.transform, false);
        
        // Agregar RectTransform
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(800, 200);
        
        // Agregar TextMeshProUGUI
        countdownText = textObj.AddComponent<TextMeshProUGUI>();
        countdownText.text = "";
        countdownText.fontSize = 120;
        countdownText.alignment = TextAlignmentOptions.Center;
        countdownText.color = Color.white;
        countdownText.fontStyle = FontStyles.Bold;
        
        // Agregar outline para mejor visibilidad
        countdownText.outlineWidth = 0.2f;
        countdownText.outlineColor = Color.black;
    }
    
    public void StartCountdown()
    {
        if (countdownActive) return;
        
        countdownActive = true;
        raceStarted = false;
        
        // Deshabilitar controles de jugadores
        DisablePlayerControls();
        
        // Iniciar coroutine del countdown
        StartCoroutine(CountdownSequence());
    }
    
    IEnumerator CountdownSequence()
    {
        // Reproducir sonido de countdown
        if (audioSource != null && countdownSound != null)
        {
            audioSource.clip = countdownSound;
            audioSource.Play();
        }
        
        // Mostrar "3"
        if (countdownText != null)
        {
            countdownText.text = "3";
            countdownText.color = Color.red;
        }
        yield return new WaitForSeconds(countdownInterval);
        
        // Mostrar "2"
        if (countdownText != null)
        {
            countdownText.text = "2";
            countdownText.color = Color.yellow;
        }
        yield return new WaitForSeconds(countdownInterval);
        
        // Mostrar "1"
        if (countdownText != null)
        {
            countdownText.text = "1";
            countdownText.color = Color.green;
        }
        yield return new WaitForSeconds(countdownInterval);
        
        // Mostrar "START"
        if (countdownText != null)
        {
            countdownText.text = "START";
            countdownText.color = Color.green;
            countdownText.fontSize = 150;
        }
        yield return new WaitForSeconds(startDisplayTime);
        
        // Ocultar texto
        if (countdownText != null)
        {
            countdownText.text = "";
        }
        
        // Habilitar controles de jugadores
        EnablePlayerControls();
        
        raceStarted = true;
        countdownActive = false;
        
        Debug.Log("✓ ¡Carrera iniciada!");
    }
    
    void DisablePlayerControls()
    {
        if (gameModeController == null) return;
        
        // Deshabilitar Player 1 (CarMovement)
        if (gameModeController.carMovementP1 != null)
        {
            gameModeController.carMovementP1.enabled = false;
        }
        
        // Deshabilitar Player 2
        if (gameModeController.carPlayer2 != null)
        {
            gameModeController.carPlayer2.enabled = false;
        }
        
        // Deshabilitar Player 3
        if (gameModeController.carPlayer3 != null)
        {
            gameModeController.carPlayer3.enabled = false;
        }
        
        // Deshabilitar Player 4
        if (gameModeController.carPlayer4 != null)
        {
            gameModeController.carPlayer4.enabled = false;
        }
        
        // Deshabilitar ArduinoSteering
        if (gameModeController.arduinoSteering != null)
        {
            gameModeController.arduinoSteering.enabled = false;
        }
        
        Debug.Log("Controles deshabilitados durante countdown");
    }
    
    void EnablePlayerControls()
    {
        if (gameModeController == null) return;
        
        // Habilitar Player 1 (CarMovement)
        if (gameModeController.carMovementP1 != null)
        {
            gameModeController.carMovementP1.enabled = true;
        }
        
        // Habilitar Player 2
        if (gameModeController.carPlayer2 != null && gameModeController.carPlayer2.gameObject.activeSelf)
        {
            gameModeController.carPlayer2.enabled = true;
        }
        
        // Habilitar Player 3
        if (gameModeController.carPlayer3 != null && gameModeController.carPlayer3.gameObject.activeSelf)
        {
            gameModeController.carPlayer3.enabled = true;
        }
        
        // Habilitar Player 4
        if (gameModeController.carPlayer4 != null && gameModeController.carPlayer4.gameObject.activeSelf)
        {
            gameModeController.carPlayer4.enabled = true;
        }
        
        // Habilitar ArduinoSteering
        if (gameModeController.arduinoSteering != null)
        {
            gameModeController.arduinoSteering.enabled = true;
        }
        
        Debug.Log("Controles habilitados - ¡Carrera iniciada!");
    }
    
    public bool IsRaceStarted()
    {
        return raceStarted;
    }
    
    public bool IsCountdownActive()
    {
        return countdownActive;
    }
}

