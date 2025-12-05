using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlsDisplayController : MonoBehaviour
{
    [Header("Panel de Controles")]
    public GameObject controlsPanel;
    
    [Header("Textos de Controles por Jugador")]
    public TextMeshProUGUI player1ControlsText;
    public TextMeshProUGUI player2ControlsText;
    public TextMeshProUGUI player3ControlsText;
    public TextMeshProUGUI player4ControlsText;
    
    [Header("Paneles de Jugadores")]
    public GameObject player1Panel;
    public GameObject player2Panel;
    public GameObject player3Panel;
    public GameObject player4Panel;
    
    [Header("Botón START")]
    public Button startButton;
    
    [Header("Referencias")]
    public MainMenu mainMenu;
    
    private int playerCount = 2;
    private int selectedLevel = 1;
    
    void Start()
    {
        // Buscar MainMenu si no está asignado
        if (mainMenu == null)
        {
            mainMenu = FindObjectOfType<MainMenu>();
        }
        
        // Crear UI si no existe
        if (controlsPanel == null)
        {
            CreateControlsUI();
        }
        
        // Configurar botón START
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        // Ocultar panel por defecto
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }
    
    void CreateControlsUI()
    {
        // Buscar Canvas existente o crear uno nuevo
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("ControlsCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Crear panel principal
        GameObject panelObj = new GameObject("ControlsPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        controlsPanel = panelObj;
        
        // Agregar Image de fondo
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f); // Fondo semi-transparente
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        // Crear contenedor central para mejor organización
        GameObject containerObj = new GameObject("ContentContainer");
        containerObj.transform.SetParent(panelObj.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.05f);
        containerRect.anchorMax = new Vector2(0.9f, 0.95f);
        containerRect.sizeDelta = Vector2.zero;
        containerRect.anchoredPosition = Vector2.zero;
        
        // Crear título
        CreateTitle(containerObj.transform);
        
        // Crear paneles de controles para cada jugador con mejor distribución
        // Distribución vertical centrada y espaciada
        player1Panel = CreatePlayerPanel(containerObj.transform, "Player1Panel", "PLAYER 1", 0.82f);
        player2Panel = CreatePlayerPanel(containerObj.transform, "Player2Panel", "PLAYER 2", 0.62f);
        player3Panel = CreatePlayerPanel(containerObj.transform, "Player3Panel", "PLAYER 3", 0.42f);
        player4Panel = CreatePlayerPanel(containerObj.transform, "Player4Panel", "PLAYER 4", 0.22f);
        
        // Crear botón START
        CreateStartButton(containerObj.transform);
        
        Debug.Log("UI de controles creada automáticamente");
    }
    
    void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.95f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.sizeDelta = new Vector2(1000, 120);
        titleRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "CONTROLES";
        titleText.fontSize = 72;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;
        titleText.outlineWidth = 0.2f;
        titleText.outlineColor = Color.black;
    }
    
    GameObject CreatePlayerPanel(Transform parent, string name, string playerName, float yPosition)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);
        
        // Panel con fondo más visible y borde
        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0.2f, 0.2f, 0.25f, 0.85f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, yPosition);
        panelRect.anchorMax = new Vector2(0.5f, yPosition);
        panelRect.sizeDelta = new Vector2(1400, 160);
        panelRect.anchoredPosition = Vector2.zero;
        
        // Título del jugador con mejor diseño
        GameObject titleObj = new GameObject("PlayerTitle");
        titleObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.25f, 1);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.offsetMin = new Vector2(25, 15);
        titleRect.offsetMax = new Vector2(-15, -15);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = playerName;
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.85f, 0f); // Amarillo dorado más brillante
        titleText.fontStyle = FontStyles.Bold;
        titleText.outlineWidth = 0.2f;
        titleText.outlineColor = Color.black;
        
        // Separador vertical
        GameObject separatorObj = new GameObject("Separator");
        separatorObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform separatorRect = separatorObj.AddComponent<RectTransform>();
        separatorRect.anchorMin = new Vector2(0.25f, 0);
        separatorRect.anchorMax = new Vector2(0.25f, 1);
        separatorRect.sizeDelta = new Vector2(3, 0);
        separatorRect.anchoredPosition = Vector2.zero;
        separatorRect.offsetMin = new Vector2(-1.5f, 20);
        separatorRect.offsetMax = new Vector2(1.5f, -20);
        
        Image separatorImage = separatorObj.AddComponent<Image>();
        separatorImage.color = new Color(1f, 0.85f, 0f, 0.6f); // Línea dorada
        
        // Texto de controles con mejor formato
        GameObject controlsObj = new GameObject("ControlsText");
        controlsObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform controlsRect = controlsObj.AddComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0.25f, 0);
        controlsRect.anchorMax = new Vector2(1, 1);
        controlsRect.sizeDelta = Vector2.zero;
        controlsRect.anchoredPosition = Vector2.zero;
        controlsRect.offsetMin = new Vector2(30, 15);
        controlsRect.offsetMax = new Vector2(-25, -15);
        
        TextMeshProUGUI controlsText = controlsObj.AddComponent<TextMeshProUGUI>();
        controlsText.fontSize = 32;
        controlsText.alignment = TextAlignmentOptions.Left;
        controlsText.color = Color.white;
        controlsText.enableWordWrapping = true;
        controlsText.lineSpacing = 15f; // Más espaciado entre líneas
        controlsText.richText = true; // Habilitar rich text para colores
        
        // Guardar referencia según el jugador
        if (name == "Player1Panel")
            player1ControlsText = controlsText;
        else if (name == "Player2Panel")
            player2ControlsText = controlsText;
        else if (name == "Player3Panel")
            player3ControlsText = controlsText;
        else if (name == "Player4Panel")
            player4ControlsText = controlsText;
        
        return panelObj;
    }
    
    void CreateStartButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.02f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.02f);
        buttonRect.sizeDelta = new Vector2(400, 100);
        buttonRect.anchoredPosition = Vector2.zero;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.1f, 0.7f, 0.1f); // Verde más oscuro
        
        startButton = buttonObj.AddComponent<Button>();
        
        // Configurar colores del botón
        ColorBlock colors = startButton.colors;
        colors.normalColor = new Color(0.1f, 0.7f, 0.1f);
        colors.highlightedColor = new Color(0.2f, 0.9f, 0.2f);
        colors.pressedColor = new Color(0.05f, 0.5f, 0.05f);
        colors.selectedColor = new Color(0.15f, 0.8f, 0.15f);
        startButton.colors = colors;
        
        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "START";
        buttonText.fontSize = 56;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.outlineWidth = 0.2f;
        buttonText.outlineColor = Color.black;
    }
    
    public void ShowControls(int players, int level)
    {
        playerCount = players;
        selectedLevel = level;
        
        // Obtener información del GameModeManager
        if (GameModeManager.Instance != null)
        {
            playerCount = GameModeManager.Instance.PlayerCount;
            selectedLevel = GameModeManager.Instance.SelectedLevel;
        }
        
        // Mostrar panel
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
        
        // Ocultar menú de selección de nivel
        if (mainMenu != null && mainMenu.levelSelectMenu != null)
        {
            mainMenu.levelSelectMenu.SetActive(false);
        }
        
        // Actualizar controles según número de jugadores
        UpdateControlsDisplay();
    }
    
    void UpdateControlsDisplay()
    {
        // Calcular posiciones dinámicas según número de jugadores
        float[] positions = CalculatePositions(playerCount);
        
        // Player 1 - Siempre activo
        if (player1Panel != null)
        {
            player1Panel.SetActive(true);
            UpdatePanelPosition(player1Panel, positions[0]);
        }
        if (player1ControlsText != null)
        {
            player1ControlsText.text = "<size=36><color=#FFD700>VOLANTE</color></size>\n" +
                                       "<size=28>• Botón <color=#00FF00>DERECHO</color>: <b>ACELERADOR</b></size>\n" +
                                       "<size=28>• Botón <color=#FF0000>IZQUIERDO</color>: <b>FRENO</b></size>";
        }
        
        // Player 2
        if (player2Panel != null)
        {
            player2Panel.SetActive(playerCount >= 2);
            if (playerCount >= 2)
                UpdatePanelPosition(player2Panel, positions[1]);
        }
        if (player2ControlsText != null && playerCount >= 2)
        {
            player2ControlsText.text = "<size=36><color=#FFD700>FLECHAS</color></size>\n" +
                                       "<size=28>• <color=#00FF00>↑</color> <b>ACELERAR</b></size>\n" +
                                       "<size=28>• <color=#FF0000>↓</color> <b>FRENAR</b></size>\n" +
                                       "<size=28>• <color=#00AAFF>← →</color> <b>GIRAR</b></size>";
        }
        
        // Player 3
        if (player3Panel != null)
        {
            player3Panel.SetActive(playerCount >= 3);
            if (playerCount >= 3)
                UpdatePanelPosition(player3Panel, positions[2]);
        }
        if (player3ControlsText != null && playerCount >= 3)
        {
            player3ControlsText.text = "<size=36><color=#FFD700>WASD</color></size>\n" +
                                       "<size=28>• <color=#00FF00>W</color> <b>ACELERAR</b></size>\n" +
                                       "<size=28>• <color=#FF0000>S</color> <b>FRENAR</b></size>\n" +
                                       "<size=28>• <color=#00AAFF>A D</color> <b>GIRAR</b></size>";
        }
        
        // Player 4
        if (player4Panel != null)
        {
            player4Panel.SetActive(playerCount >= 4);
            if (playerCount >= 4)
                UpdatePanelPosition(player4Panel, positions[3]);
        }
        if (player4ControlsText != null && playerCount >= 4)
        {
            player4ControlsText.text = "<size=36><color=#FFD700>IJKL</color></size>\n" +
                                       "<size=28>• <color=#00FF00>I</color> <b>ACELERAR</b></size>\n" +
                                       "<size=28>• <color=#FF0000>K</color> <b>FRENAR</b></size>\n" +
                                       "<size=28>• <color=#00AAFF>J L</color> <b>GIRAR</b></size>";
        }
    }
    
    float[] CalculatePositions(int count)
    {
        // Distribución centrada según número de jugadores
        float[] positions = new float[4];
        
        switch (count)
        {
            case 2:
                positions[0] = 0.65f; // Player 1
                positions[1] = 0.35f; // Player 2
                positions[2] = 0f;
                positions[3] = 0f;
                break;
            case 3:
                positions[0] = 0.75f; // Player 1
                positions[1] = 0.50f; // Player 2
                positions[2] = 0.25f; // Player 3
                positions[3] = 0f;
                break;
            case 4:
                positions[0] = 0.82f; // Player 1
                positions[1] = 0.62f; // Player 2
                positions[2] = 0.42f; // Player 3
                positions[3] = 0.22f; // Player 4
                break;
            default:
                positions[0] = 0.5f;
                positions[1] = 0.5f;
                positions[2] = 0.5f;
                positions[3] = 0.5f;
                break;
        }
        
        return positions;
    }
    
    void UpdatePanelPosition(GameObject panel, float yPosition)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, yPosition);
            rect.anchorMax = new Vector2(0.5f, yPosition);
        }
    }
    
    public void OnStartButtonClicked()
    {
        // Cargar la escena del nivel seleccionado
        LoadGameScene();
    }
    
    private void LoadGameScene()
    {
        string sceneName = "Level_01"; // Por defecto
        
        if (GameModeManager.Instance != null)
        {
            sceneName = GameModeManager.Instance.GetLevelSceneName();
        }
        else
        {
            // Fallback si no hay GameModeManager
            sceneName = "Level_0" + selectedLevel;
        }
        
        Debug.Log("Cargando escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    
    public void HideControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }
}

