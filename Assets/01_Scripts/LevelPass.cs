using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPass : MonoBehaviour
{
	private WinnerVideoPlayer winnerVideoPlayer;
	private bool gameEnded = false;

	void Start()
	{
		Debug.Log("LevelPass: Iniciando...");
		
		// Verificar que tiene Collider2D
		Collider2D col = GetComponent<Collider2D>();
		if (col == null)
		{
			Debug.LogError("LevelPass: ¡ERROR! No hay Collider2D en el objeto Goal. Agrega un Collider2D con 'Is Trigger' activado.");
		}
		else
		{
			if (!col.isTrigger)
			{
				Debug.LogWarning("LevelPass: El Collider2D no está marcado como 'Is Trigger'. Activándolo automáticamente...");
				col.isTrigger = true;
			}
			Debug.Log("LevelPass: Collider2D configurado correctamente. Is Trigger: " + col.isTrigger);
		}
		
		// Buscar el WinnerVideoPlayer en la escena
		winnerVideoPlayer = FindObjectOfType<WinnerVideoPlayer>();
		
		if (winnerVideoPlayer == null)
		{
			Debug.LogWarning("LevelPass: WinnerVideoPlayer no encontrado en la escena. Creando uno...");
			GameObject videoPlayerObj = new GameObject("WinnerVideoPlayer");
			winnerVideoPlayer = videoPlayerObj.AddComponent<WinnerVideoPlayer>();
		}
		else
		{
			Debug.Log("LevelPass: WinnerVideoPlayer encontrado correctamente.");
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("LevelPass: OnTriggerEnter2D detectado! Objeto: " + collision.gameObject.name + ", Tag: " + collision.tag);
		
		if (collision.CompareTag("Player") && !gameEnded)
		{
			Debug.Log("LevelPass: ¡Auto ganador detectado! " + collision.gameObject.name);
			gameEnded = true;
			
			// Obtener el GameObject del auto ganador
			GameObject winnerCar = collision.gameObject;
			
			// Reproducir el video del ganador
			if (winnerVideoPlayer != null)
			{
				Debug.Log("LevelPass: Llamando a PlayWinnerVideo...");
				winnerVideoPlayer.PlayWinnerVideo(winnerCar);
			}
			else
			{
				Debug.LogError("LevelPass: WinnerVideoPlayer no está disponible. Yendo directamente al menú principal.");
				// Fallback: ir directamente al menú principal
				SceneManager.LoadScene("Start_Panel 1");
			}
		}
		else
		{
			if (gameEnded)
			{
				Debug.Log("LevelPass: El juego ya terminó, ignorando trigger.");
			}
			else if (!collision.CompareTag("Player"))
			{
				Debug.LogWarning("LevelPass: El objeto que entró no tiene el tag 'Player'. Tag actual: " + collision.tag);
			}
		}
	}
}