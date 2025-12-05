using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Referencias de Cámaras")]
    public Camera cameraP1;
    public Camera cameraP2;
    public Camera cameraP3;
    public Camera cameraP4;

    [Header("Referencias de Jugadores")]
    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;

    [Header("Cámara Principal (opcional)")]
    public Camera mainCamera;

    [Header("Cámara de Fondo Negro (para 3 jugadores)")]
    private Camera backgroundCamera;

    [Header("Configuración")]
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothness = 0.1f;
    
    [Header("UI Player Labels")]
    private TextMeshProUGUI player1Label;
    private TextMeshProUGUI player2Label;
    private TextMeshProUGUI player3Label;
    private TextMeshProUGUI player4Label;

    private Vector3 velocity1 = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;
    private Vector3 velocity3 = Vector3.zero;
    private Vector3 velocity4 = Vector3.zero;

    private int playerCount = 2;

    void Start()
    {
        // Desactivar main camera si existe para evitar conflicto
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        // Obtener el número de jugadores del GameModeManager
        if (GameModeManager.Instance != null)
        {
            playerCount = GameModeManager.Instance.PlayerCount;
        }

        // Crear UI labels para cada jugador
        CreatePlayerLabels();
        
        ConfigureCameras();
    }

    void Update()
    {
        UpdateCameraPositions();
    }

    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 2, 4);
        ConfigureCameras();
    }

    void ConfigureCameras()
    {
        // Desactivar todas las cámaras primero
        if (cameraP3 != null) cameraP3.gameObject.SetActive(false);
        if (cameraP4 != null) cameraP4.gameObject.SetActive(false);
        
        // Desactivar cámara de fondo negro si existe
        if (backgroundCamera != null)
        {
            backgroundCamera.gameObject.SetActive(false);
        }

        switch (playerCount)
        {
            case 2:
                ConfigureTwoPlayers();
                break;
            case 3:
                ConfigureThreePlayers();
                break;
            case 4:
                ConfigureFourPlayers();
                break;
        }
    }

    void ConfigureTwoPlayers()
    {
        if (cameraP1 == null || cameraP2 == null)
        {
            Debug.LogError("SplitScreenManager: Faltan CameraP1 y/o CameraP2");
            return;
        }

        // División horizontal - mitad superior e inferior
        cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);  // Arriba
        cameraP2.rect = new Rect(0, 0, 1, 0.5f);     // Abajo

        cameraP1.gameObject.SetActive(true);
        cameraP2.gameObject.SetActive(true);

        // Actualizar posiciones de labels
        UpdatePlayerLabel(player1Label, cameraP1.rect, "PLAYER 1");
        UpdatePlayerLabel(player2Label, cameraP2.rect, "PLAYER 2");
        // Ocultar completamente los labels no usados (incluyendo el objeto padre)
        if (player3Label != null && player3Label.transform.parent != null)
            player3Label.transform.parent.gameObject.SetActive(false);
        if (player4Label != null && player4Label.transform.parent != null)
            player4Label.transform.parent.gameObject.SetActive(false);

        // Asegurar que hay un AudioListener
        EnsureAudioListener(cameraP1);

        Debug.Log("✓ Split Screen configurado para 2 jugadores");
    }

    void ConfigureThreePlayers()
    {
        if (cameraP1 == null || cameraP2 == null || cameraP3 == null)
        {
            Debug.LogError("SplitScreenManager: Faltan cámaras para 3 jugadores");
            return;
        }

        // P1 arriba izquierda, P2 arriba derecha, P3 abajo centrado
        cameraP1.rect = new Rect(0, 0.5f, 0.5f, 0.5f);      // Arriba izquierda
        cameraP2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);   // Arriba derecha
        cameraP3.rect = new Rect(0.25f, 0, 0.5f, 0.5f);     // Abajo centrado

        cameraP1.gameObject.SetActive(true);
        cameraP2.gameObject.SetActive(true);
        cameraP3.gameObject.SetActive(true);

        // Actualizar posiciones de labels
        UpdatePlayerLabel(player1Label, cameraP1.rect, "PLAYER 1");
        UpdatePlayerLabel(player2Label, cameraP2.rect, "PLAYER 2");
        UpdatePlayerLabel(player3Label, cameraP3.rect, "PLAYER 3");
        // Ocultar completamente el label no usado (incluyendo el objeto padre)
        if (player4Label != null && player4Label.transform.parent != null)
            player4Label.transform.parent.gameObject.SetActive(false);

        // Crear o activar cámara de fondo negro para cubrir áreas vacías
        CreateBackgroundCamera();

        // Asegurar que hay un AudioListener
        EnsureAudioListener(cameraP1);

        Debug.Log("✓ Split Screen configurado para 3 jugadores");
    }

    void ConfigureFourPlayers()
    {
        if (cameraP1 == null || cameraP2 == null || cameraP3 == null || cameraP4 == null)
        {
            Debug.LogError("SplitScreenManager: Faltan cámaras para 4 jugadores");
            return;
        }

        // Cuadrícula 2x2
        cameraP1.rect = new Rect(0, 0.5f, 0.5f, 0.5f);      // Arriba izquierda
        cameraP2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);   // Arriba derecha
        cameraP3.rect = new Rect(0, 0, 0.5f, 0.5f);         // Abajo izquierda
        cameraP4.rect = new Rect(0.5f, 0, 0.5f, 0.5f);      // Abajo derecha

        cameraP1.gameObject.SetActive(true);
        cameraP2.gameObject.SetActive(true);
        cameraP3.gameObject.SetActive(true);
        cameraP4.gameObject.SetActive(true);

        // Actualizar posiciones de labels
        UpdatePlayerLabel(player1Label, cameraP1.rect, "PLAYER 1");
        UpdatePlayerLabel(player2Label, cameraP2.rect, "PLAYER 2");
        UpdatePlayerLabel(player3Label, cameraP3.rect, "PLAYER 3");
        UpdatePlayerLabel(player4Label, cameraP4.rect, "PLAYER 4");

        // Asegurar que hay un AudioListener
        EnsureAudioListener(cameraP1);

        Debug.Log("✓ Split Screen configurado para 4 jugadores");
    }

    void UpdateCameraPositions()
    {
        // Actualizar posición de CameraP1
        if (player1 != null && cameraP1 != null && cameraP1.gameObject.activeSelf)
        {
            Vector3 targetPos1 = player1.position + cameraOffset;
            cameraP1.transform.position = Vector3.SmoothDamp(
                cameraP1.transform.position,
                targetPos1,
                ref velocity1,
                cameraSmoothness
            );
        }

        // Actualizar posición de CameraP2
        if (player2 != null && cameraP2 != null && cameraP2.gameObject.activeSelf)
        {
            Vector3 targetPos2 = player2.position + cameraOffset;
            cameraP2.transform.position = Vector3.SmoothDamp(
                cameraP2.transform.position,
                targetPos2,
                ref velocity2,
                cameraSmoothness
            );
        }

        // Actualizar posición de CameraP3
        if (player3 != null && cameraP3 != null && cameraP3.gameObject.activeSelf)
        {
            Vector3 targetPos3 = player3.position + cameraOffset;
            cameraP3.transform.position = Vector3.SmoothDamp(
                cameraP3.transform.position,
                targetPos3,
                ref velocity3,
                cameraSmoothness
            );
        }

        // Actualizar posición de CameraP4
        if (player4 != null && cameraP4 != null && cameraP4.gameObject.activeSelf)
        {
            Vector3 targetPos4 = player4.position + cameraOffset;
            cameraP4.transform.position = Vector3.SmoothDamp(
                cameraP4.transform.position,
                targetPos4,
                ref velocity4,
                cameraSmoothness
            );
        }
    }

    // Método para ajustar el offset dinámicamente
    public void SetCameraOffset(Vector3 newOffset)
    {
        cameraOffset = newOffset;
    }

    // Método para ajustar la suavidad dinámicamente
    public void SetSmoothness(float newSmoothness)
    {
        cameraSmoothness = newSmoothness;
    }

    // Crear cámara de fondo negro para cubrir áreas vacías en modo de 3 jugadores
    private void CreateBackgroundCamera()
    {
        // Si la cámara de fondo ya existe, solo activarla
        if (backgroundCamera != null)
        {
            backgroundCamera.gameObject.SetActive(true);
            return;
        }

        // Crear nuevo GameObject para la cámara de fondo
        GameObject bgCameraObj = new GameObject("BackgroundCamera_Black");
        backgroundCamera = bgCameraObj.AddComponent<Camera>();
        
        // Configurar la cámara para cubrir toda la pantalla con fondo negro
        backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
        backgroundCamera.backgroundColor = Color.black;
        backgroundCamera.cullingMask = 0; // No renderizar nada, solo mostrar el color de fondo
        backgroundCamera.depth = -1; // Profundidad más baja para que esté detrás de todas las demás
        backgroundCamera.rect = new Rect(0, 0, 1, 1); // Cubrir toda la pantalla
        
        // Desactivar componentes innecesarios
        if (backgroundCamera.GetComponent<AudioListener>() != null)
        {
            Destroy(backgroundCamera.GetComponent<AudioListener>());
        }
        
        Debug.Log("✓ Cámara de fondo negro creada para modo de 3 jugadores");
    }

    // Asegurar que hay un AudioListener en la escena
    private void EnsureAudioListener(Camera camera)
    {
        if (camera == null) return;

        // Verificar si ya hay un AudioListener en alguna cámara activa
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        
        // Si no hay ningún AudioListener, agregar uno a la primera cámara
        if (listeners.Length == 0)
        {
            if (camera.GetComponent<AudioListener>() == null)
            {
                camera.gameObject.AddComponent<AudioListener>();
                Debug.Log("AudioListener agregado a " + camera.name);
            }
        }
        else
        {
            // Si hay AudioListeners, asegurarse de que al menos uno esté en una cámara activa
            bool hasActiveListener = false;
            foreach (AudioListener listener in listeners)
            {
                if (listener.gameObject.activeInHierarchy)
                {
                    hasActiveListener = true;
                    break;
                }
            }

            // Si no hay AudioListener activo, agregar uno a la cámara especificada
            if (!hasActiveListener)
            {
                if (camera.GetComponent<AudioListener>() == null)
                {
                    camera.gameObject.AddComponent<AudioListener>();
                    Debug.Log("AudioListener agregado a " + camera.name + " (ningún otro estaba activo)");
                }
            }
        }
    }
    
    // Crear labels de jugadores para cada cámara
    void CreatePlayerLabels()
    {
        // Buscar Canvas existente o crear uno nuevo
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PlayerLabelsCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Asegurar que esté por encima de todo
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Crear labels para cada jugador
        player1Label = CreatePlayerLabel(canvas.transform, "Player1Label", "PLAYER 1");
        player2Label = CreatePlayerLabel(canvas.transform, "Player2Label", "PLAYER 2");
        player3Label = CreatePlayerLabel(canvas.transform, "Player3Label", "PLAYER 3");
        player4Label = CreatePlayerLabel(canvas.transform, "Player4Label", "PLAYER 4");
        
        // Ocultar todos inicialmente (ocultar el objeto padre completo que incluye el fondo)
        if (player1Label != null && player1Label.transform.parent != null)
            player1Label.transform.parent.gameObject.SetActive(false);
        if (player2Label != null && player2Label.transform.parent != null)
            player2Label.transform.parent.gameObject.SetActive(false);
        if (player3Label != null && player3Label.transform.parent != null)
            player3Label.transform.parent.gameObject.SetActive(false);
        if (player4Label != null && player4Label.transform.parent != null)
            player4Label.transform.parent.gameObject.SetActive(false);
    }
    
    TextMeshProUGUI CreatePlayerLabel(Transform parent, string name, string text)
    {
        GameObject labelObj = new GameObject(name);
        labelObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = labelObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(250, 50);
        
        // Agregar fondo semi-transparente primero
        Image bgImage = labelObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        
        // Crear objeto hijo para el texto
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(labelObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI labelText = textObj.AddComponent<TextMeshProUGUI>();
        labelText.text = text;
        labelText.fontSize = 32;
        labelText.alignment = TextAlignmentOptions.Left; // Alineación izquierda para texto horizontal
        labelText.color = Color.white;
        labelText.fontStyle = FontStyles.Bold;
        labelText.outlineWidth = 0.3f;
        labelText.outlineColor = Color.black;
        labelText.enableWordWrapping = false; // Asegurar que el texto sea horizontal
        
        return labelText;
    }
    
    void UpdatePlayerLabel(TextMeshProUGUI label, Rect cameraRect, string text)
    {
        if (label == null) return;
        
        label.text = text;
        
        // Obtener el RectTransform del objeto padre (el que tiene el Image de fondo)
        RectTransform rect = label.transform.parent.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = label.GetComponent<RectTransform>();
        }
        
        if (rect != null)
        {
            // cameraRect: x (izquierda), y (abajo), width, height
            // En Camera.rect, y=0 es abajo, y=1 es arriba
            // Calcular la esquina superior izquierda del viewport de la cámara
            float viewportLeft = cameraRect.x;
            float viewportTop = cameraRect.y + cameraRect.height; // Y + altura = parte superior (0-1)
            
            // Usar anchors para posicionar relativo a la pantalla completa
            // Convertir coordenadas del viewport (0-1) directamente a anchors
            rect.anchorMin = new Vector2(viewportLeft, viewportTop);
            rect.anchorMax = new Vector2(viewportLeft, viewportTop);
            rect.pivot = new Vector2(0, 1); // Pivot en esquina superior izquierda del label
            
            // Offset desde la esquina del viewport (en píxeles)
            // El CanvasScaler ajustará automáticamente según la resolución
            float offsetX = 20f;
            float offsetY = -20f;
            
            rect.anchoredPosition = new Vector2(offsetX, offsetY);
            rect.sizeDelta = new Vector2(250, 50);
            
            // Activar el objeto completo (padre con fondo + texto)
            rect.gameObject.SetActive(true);
        }
    }
}
