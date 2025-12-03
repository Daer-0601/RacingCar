using UnityEngine;

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

    [Header("Configuración")]
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothness = 0.1f;

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
}
