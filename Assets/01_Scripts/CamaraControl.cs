using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
	public float[] Rotz;
	int triggerCount = 0;

	Camera cam;

	bool CanRotate;

	public float rotSpeed;

	void Start()
	{
		cam = Camera.main;
	}

	void FixedUpdate()
	{
		if (CanRotate)
		{
			Quaternion camRot = cam.transform.rotation;
			Quaternion desRot = Quaternion.Euler(0, 0, Rotz[triggerCount - 1]);

			cam.transform.rotation = Quaternion.Lerp(camRot, desRot, Time.deltaTime * rotSpeed);

			// Detener la rotación cuando esté cerca del ángulo objetivo
			if (Quaternion.Angle(camRot, desRot) < 0.1f)
			{
				CanRotate = false;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("hit"); // Corregido: "log" -> "Log"

		if (triggerCount == 4) // Asumiendo que Rotz tiene 4 elementos
			triggerCount = 0;

		triggerCount += 1;

		// Verificar que el índice sea válido
		if (triggerCount - 1 < Rotz.Length && triggerCount - 1 >= 0)
		{
			CanRotate = true;
		}
		else
		{
			Debug.LogWarning("Índice fuera de rango en Rotz: " + (triggerCount - 1));
		}
	}
}