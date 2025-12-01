using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Referencias")]
    public Camera cameraP1;
    public Camera cameraP2;
    public Transform player1;
    public Transform player2;

    [Header("Cámara Principal (opcional)")]
    public Camera mainCamera;

    [Header("Configuración")]
    public bool verticalSplit = false;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothness = 0.1f;

    private Vector3 velocity1 = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;

    void Start()
    {
        // Desactivar main camera si existe para evitar conflicto
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        // Verificar que tenemos las cámaras necesarias
        if (cameraP1 == null || cameraP2 == null)
        {
            Debug.LogError("SplitScreenManager: Faltan asignar CameraP1 y/o CameraP2 en el Inspector");
        }
    }

    void Update()
    {
        ConfigureSplitScreen();
        UpdateCameraPositions();
    }

    void ConfigureSplitScreen()
    {
        if (cameraP1 == null || cameraP2 == null) return;

        if (verticalSplit)
        {
            // División vertical - mitad izquierda y derecha
            cameraP1.rect = new Rect(0, 0, 0.5f, 1);
            cameraP2.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else
        {
            // División horizontal - mitad superior e inferior
            cameraP1.rect = new Rect(0, 0.5f, 1, 0.5f);
            cameraP2.rect = new Rect(0, 0, 1, 0.5f);
        }
    }

    void UpdateCameraPositions()
    {
        // Actualizar posición de CameraP1
        if (player1 != null && cameraP1 != null)
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
        if (player2 != null && cameraP2 != null)
        {
            Vector3 targetPos2 = player2.position + cameraOffset;
            cameraP2.transform.position = Vector3.SmoothDamp(
                cameraP2.transform.position,
                targetPos2,
                ref velocity2,
                cameraSmoothness
            );
        }
    }

    // Método para cambiar entre división horizontal/vertical en tiempo de ejecución
    public void ToggleSplitOrientation()
    {
        verticalSplit = !verticalSplit;
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