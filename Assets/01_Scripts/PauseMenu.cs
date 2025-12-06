using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel de Pausa")]
    public GameObject pausePanel;
    
    [Header("Botones")]
    public Button resumeButton;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Configuración")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    private bool isPaused = false;
    private Canvas pauseCanvas;
    private EventSystem eventSystem;
    
    void Start()
    {
        // Asegurar que hay un EventSystem para los botones
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
        
        // Crear UI si no existe
        if (pausePanel == null)
        {
            CreatePauseUI();
        }
        
        // Configurar botones
        SetupButtons();
        
        // Ocultar panel por defecto
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Detectar tecla ESC para pausar/reanudar
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    void CreatePauseUI()
    {
        // Buscar Canvas existente o crear uno nuevo
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PauseMenuCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200; // Asegurar que esté por encima de todo
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
            pauseCanvas = canvas;
        }
        else
        {
            pauseCanvas = canvas;
        }
        
        // Crear panel principal
        GameObject panelObj = new GameObject("PausePanel");
        panelObj.transform.SetParent(pauseCanvas.transform, false);
        pausePanel = panelObj;
        
        // Agregar Image de fondo
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f); // Fondo semi-transparente
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        // Crear título
        CreateTitle(panelObj.transform);
        
        // Crear botones
        CreateButtons(panelObj.transform);
        
        Debug.Log("UI de pausa creada automáticamente");
    }
    
    void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(600, 100);
        titleRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "PAUSA";
        titleText.fontSize = 64;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;
        titleText.outlineWidth = 0.3f;
        titleText.outlineColor = Color.black;
    }
    
    void CreateButtons(Transform parent)
    {
        // Contenedor para botones
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(parent, false);
        
        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.4f);
        containerRect.anchorMax = new Vector2(0.5f, 0.4f);
        containerRect.sizeDelta = new Vector2(400, 300);
        containerRect.anchoredPosition = Vector2.zero;
        
        // Botón Reanudar
        resumeButton = CreateButton(buttonContainer.transform, "ResumeButton", "REANUDAR", 0.66f, new Color(0.2f, 0.7f, 0.2f));
        
        // Botón Reiniciar
        restartButton = CreateButton(buttonContainer.transform, "RestartButton", "REINICIAR", 0.33f, new Color(0.7f, 0.5f, 0.2f));
        
        // Botón Menú Principal
        mainMenuButton = CreateButton(buttonContainer.transform, "MainMenuButton", "MENÚ PRINCIPAL", 0f, new Color(0.7f, 0.2f, 0.2f));
    }
    
    Button CreateButton(Transform parent, string name, string text, float yPosition, Color buttonColor)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, yPosition);
        buttonRect.anchorMax = new Vector2(0.5f, yPosition);
        buttonRect.sizeDelta = new Vector2(350, 70);
        buttonRect.anchoredPosition = Vector2.zero;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonColor;
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Configurar colores del botón
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = new Color(Mathf.Min(buttonColor.r * 1.2f, 1f), Mathf.Min(buttonColor.g * 1.2f, 1f), Mathf.Min(buttonColor.b * 1.2f, 1f));
        colors.pressedColor = new Color(Mathf.Max(buttonColor.r * 0.8f, 0f), Mathf.Max(buttonColor.g * 0.8f, 0f), Mathf.Max(buttonColor.b * 0.8f, 0f));
        colors.selectedColor = buttonColor;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        
        // Asegurar que el botón sea interactuable
        button.interactable = true;
        
        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 32;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.outlineWidth = 0.2f;
        buttonText.outlineColor = Color.black;
        
        return button;
    }
    
    void SetupButtons()
    {
        // Limpiar listeners previos para evitar duplicados
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => StartCoroutine(RestartLevelCoroutine()));
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(() => StartCoroutine(GoToMainMenuCoroutine()));
        }
    }
    
    public void PauseGame()
    {
        if (isPaused) return;
        
        isPaused = true;
        Time.timeScale = 0f; // Pausar el tiempo del juego
        
        // Mostrar panel de pausa
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        // Deshabilitar controles de jugadores
        DisablePlayerControls();
        
        Debug.Log("Juego pausado");
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        isPaused = false;
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        
        // Ocultar panel de pausa
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Habilitar controles de jugadores
        EnablePlayerControls();
        
        Debug.Log("Juego reanudado");
    }
    
    public void RestartLevel()
    {
        StartCoroutine(RestartLevelCoroutine());
    }
    
    IEnumerator RestartLevelCoroutine()
    {
        // Reanudar el tiempo antes de cargar la escena
        Time.timeScale = 1f;
        isPaused = false;
        
        // Esperar un frame para asegurar que todo se actualice
        yield return null;
        
        // Obtener el nombre de la escena actual
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Verificar que la escena existe
        if (string.IsNullOrEmpty(currentScene))
        {
            Debug.LogError("No se pudo obtener el nombre de la escena actual");
            yield break;
        }
        
        Debug.Log("Reiniciando nivel: " + currentScene);
        
        // Cargar la escena de forma síncrona
        try
        {
            SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al cargar la escena: " + e.Message);
        }
    }
    
    public void GoToMainMenu()
    {
        StartCoroutine(GoToMainMenuCoroutine());
    }
    
    IEnumerator GoToMainMenuCoroutine()
    {
        // Reanudar el tiempo antes de cargar la escena
        Time.timeScale = 1f;
        isPaused = false;
        
        // Esperar un frame para asegurar que todo se actualice
        yield return null;
        
        // Cargar el menú principal
        Debug.Log("Volviendo al menú principal");
        
        try
        {
            SceneManager.LoadScene("Start_Panel 1", LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al cargar el menú principal: " + e.Message);
            // Intentar con el índice de la escena si el nombre falla
            try
            {
                SceneManager.LoadScene(0);
            }
            catch (System.Exception e2)
            {
                Debug.LogError("Error al cargar escena por índice: " + e2.Message);
            }
        }
    }
    
    void DisablePlayerControls()
    {
        // Deshabilitar controles de jugadores
        GameModeController gameModeController = FindObjectOfType<GameModeController>();
        if (gameModeController != null)
        {
            if (gameModeController.carMovementP1 != null)
                gameModeController.carMovementP1.enabled = false;
            
            if (gameModeController.carPlayer2 != null)
                gameModeController.carPlayer2.enabled = false;
            
            if (gameModeController.carPlayer3 != null)
                gameModeController.carPlayer3.enabled = false;
            
            if (gameModeController.carPlayer4 != null)
                gameModeController.carPlayer4.enabled = false;
            
            if (gameModeController.arduinoSteering != null)
                gameModeController.arduinoSteering.enabled = false;
        }
    }
    
    void EnablePlayerControls()
    {
        // Habilitar controles de jugadores
        GameModeController gameModeController = FindObjectOfType<GameModeController>();
        if (gameModeController != null)
        {
            if (gameModeController.carMovementP1 != null)
                gameModeController.carMovementP1.enabled = true;
            
            if (gameModeController.carPlayer2 != null && gameModeController.carPlayer2.gameObject.activeSelf)
                gameModeController.carPlayer2.enabled = true;
            
            if (gameModeController.carPlayer3 != null && gameModeController.carPlayer3.gameObject.activeSelf)
                gameModeController.carPlayer3.enabled = true;
            
            if (gameModeController.carPlayer4 != null && gameModeController.carPlayer4.gameObject.activeSelf)
                gameModeController.carPlayer4.enabled = true;
            
            if (gameModeController.arduinoSteering != null)
                gameModeController.arduinoSteering.enabled = true;
        }
    }
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se reanude si el objeto se destruye
        Time.timeScale = 1f;
    }
}

