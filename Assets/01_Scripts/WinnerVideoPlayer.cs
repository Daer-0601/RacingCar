using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinnerVideoPlayer : MonoBehaviour
{
    [Header("Videos de Ganador")]
    public VideoClip videoPlayer1; // Video para Auto 1 (CarMovement)
    public VideoClip videoPlayer2; // Video para Auto 2 (CarPlayer2)
    public VideoClip videoPlayer3; // Video para Auto 3 (CarPlayer3)
    public VideoClip videoPlayer4; // Video para Auto 4 (CarPlayer4)

    [Header("Componentes")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas; // Canvas para mostrar el video

    [Header("Configuración")]
    public string mainMenuSceneName = "Start_Panel 1";

    private bool videoPlaying = false;

    void Start()
    {
        // Buscar VideoPlayer si no está asignado
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                // Buscar en hijos
                videoPlayer = GetComponentInChildren<VideoPlayer>();
            }
        }

        // Si aún no hay VideoPlayer, crear uno automáticamente
        if (videoPlayer == null)
        {
            GameObject videoObj = new GameObject("VideoPlayer");
            videoObj.transform.SetParent(transform);
            videoPlayer = videoObj.AddComponent<VideoPlayer>();
            Debug.Log("VideoPlayer creado automáticamente en: " + gameObject.name);
        }

        // Configurar VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            
            // Crear RenderTexture si no existe
            if (videoPlayer.targetTexture == null)
            {
                RenderTexture rt = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
                rt.name = "VideoRenderTexture";
                videoPlayer.targetTexture = rt;
                Debug.Log("RenderTexture creado automáticamente para el video");
            }
            
            // Suscribirse al evento de fin de video
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        // Buscar Canvas si no está asignado
        if (videoCanvas == null)
        {
            // Buscar Canvas en la escena
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name.Contains("Video") || canvas.name.Contains("Winner"))
                {
                    videoCanvas = canvas.gameObject;
                    break;
                }
            }
        }

        // Si no hay Canvas, crear uno automáticamente
        if (videoCanvas == null)
        {
            CreateVideoCanvas();
        }

        // Ocultar canvas de video inicialmente
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }
    }

    // Identificar qué auto llegó al Goal y reproducir su video
    public void PlayWinnerVideo(GameObject winnerCar)
    {
        Debug.Log("WinnerVideoPlayer: PlayWinnerVideo llamado para: " + winnerCar.name);
        
        if (videoPlaying)
        {
            Debug.LogWarning("WinnerVideoPlayer: Ya hay un video reproduciéndose");
            return;
        }

        VideoClip videoToPlay = null;
        int playerNumber = 0;

        // Identificar qué auto es según sus componentes
        if (winnerCar.GetComponent<CarMovement>() != null)
        {
            videoToPlay = videoPlayer1;
            playerNumber = 1;
            Debug.Log("WinnerVideoPlayer: Ganador: Auto 1 (CarMovement)");
        }
        else if (winnerCar.GetComponent<CarPlayer2>() != null)
        {
            videoToPlay = videoPlayer2;
            playerNumber = 2;
            Debug.Log("WinnerVideoPlayer: Ganador: Auto 2 (CarPlayer2)");
        }
        else if (winnerCar.GetComponent<CarPlayer3>() != null)
        {
            videoToPlay = videoPlayer3;
            playerNumber = 3;
            Debug.Log("WinnerVideoPlayer: Ganador: Auto 3 (CarPlayer3)");
        }
        else if (winnerCar.GetComponent<CarPlayer4>() != null)
        {
            videoToPlay = videoPlayer4;
            playerNumber = 4;
            Debug.Log("WinnerVideoPlayer: Ganador: Auto 4 (CarPlayer4)");
        }
        else
        {
            Debug.LogWarning("WinnerVideoPlayer: No se pudo identificar el tipo de auto. Componentes encontrados:");
            Component[] components = winnerCar.GetComponents<Component>();
            foreach (Component comp in components)
            {
                Debug.Log("  - " + comp.GetType().Name);
            }
        }

        if (videoToPlay == null)
        {
            Debug.LogWarning("WinnerVideoPlayer: No se encontró video para el auto ganador. Video asignado: " + (videoToPlay != null ? videoToPlay.name : "NULL"));
            Debug.LogWarning("WinnerVideoPlayer: Videos configurados - P1: " + (videoPlayer1 != null ? videoPlayer1.name : "NULL") + 
                           ", P2: " + (videoPlayer2 != null ? videoPlayer2.name : "NULL") + 
                           ", P3: " + (videoPlayer3 != null ? videoPlayer3.name : "NULL") + 
                           ", P4: " + (videoPlayer4 != null ? videoPlayer4.name : "NULL"));
            Debug.LogWarning("WinnerVideoPlayer: Yendo directamente al menú principal.");
            LoadMainMenu();
            return;
        }

        Debug.Log("WinnerVideoPlayer: Video encontrado: " + videoToPlay.name + " para Auto " + playerNumber);
        // Reproducir el video
        StartCoroutine(PlayVideoCoroutine(videoToPlay, playerNumber));
    }

    private IEnumerator PlayVideoCoroutine(VideoClip video, int playerNumber)
    {
        videoPlaying = true;

        // Configurar RenderTexture y RawImage si es necesario
        SetupVideoDisplay();

        // Mostrar canvas de video
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(true);
        }

        // Configurar y reproducir video
        if (videoPlayer != null && video != null)
        {
            videoPlayer.clip = video;
            videoPlayer.Prepare();

            // Esperar a que el video esté preparado
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            Debug.Log($"Reproduciendo video del ganador: Auto {playerNumber}");
            videoPlayer.Play();

            // Esperar a que termine el video
            while (videoPlayer.isPlaying)
            {
                yield return null;
            }
        }

        // El video terminó, ir al menú principal
        OnVideoFinished(videoPlayer);
    }

    private void SetupVideoDisplay()
    {
        // Si hay canvas, asegurarse de que tiene un RawImage con el RenderTexture
        if (videoCanvas != null && videoPlayer != null && videoPlayer.targetTexture != null)
        {
            UnityEngine.UI.RawImage rawImage = videoCanvas.GetComponentInChildren<UnityEngine.UI.RawImage>();
            if (rawImage == null)
            {
                // Buscar en todos los hijos
                rawImage = videoCanvas.GetComponentInChildren<UnityEngine.UI.RawImage>(true);
            }
            
            if (rawImage != null)
            {
                rawImage.texture = videoPlayer.targetTexture;
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Cargando menú principal...");
        videoPlaying = false;

        // Ocultar canvas de video
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }

        // Cargar menú principal
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        try
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar la escena del menú principal: {e.Message}");
            // Intentar cargar por índice si falla por nombre
            SceneManager.LoadScene(0);
        }
    }

    private void CreateVideoCanvas()
    {
        // Crear Canvas
        GameObject canvasObj = new GameObject("VideoCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Por encima de todo
        
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Crear RawImage para mostrar el video
        GameObject rawImageObj = new GameObject("VideoRawImage");
        rawImageObj.transform.SetParent(canvasObj.transform, false);
        
        UnityEngine.UI.RawImage rawImage = rawImageObj.AddComponent<UnityEngine.UI.RawImage>();
        RectTransform rectTransform = rawImageObj.GetComponent<RectTransform>();
        
        // Configurar para llenar toda la pantalla
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        
        // Asignar RenderTexture al RawImage
        if (videoPlayer != null && videoPlayer.targetTexture != null)
        {
            rawImage.texture = videoPlayer.targetTexture;
        }
        
        videoCanvas = canvasObj;
        Debug.Log("Canvas de video creado automáticamente");
    }

    void OnDestroy()
    {
        // Desuscribirse del evento
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}

