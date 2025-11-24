using UnityEngine;

public class CarMovement : MonoBehaviour
{
	public float MaxSpeed = 10f;   // Velocidad máxima
	public float acc = 5f;         // Fuerza de aceleración
	public float steering = 200f;  // Velocidad de giro
	public float normalDrag = 0.5f; // resistencia normal
	public float stopDrag = 5f;     // resistencia al soltar W

	private Rigidbody2D rb;
	private float X; // dirección (giro)
	private float Y; // aceleración

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.drag = normalDrag;
	}

	private void Update()
	{
		HandleInput();
	}

	private void FixedUpdate()
	{
		// Aplicar fuerza hacia adelante (solo con W/S)
		Vector2 forwardMove = transform.up * (Y * acc);
		rb.AddForce(forwardMove);

		// Rotar el coche con A/D
		if (Mathf.Abs(Y) > 0.1f) // solo gira si está acelerando
		{
			rb.rotation += X * steering * Time.fixedDeltaTime;
		}

		// Limitar velocidad máxima
		if (rb.velocity.magnitude > MaxSpeed)
		{
			rb.velocity = rb.velocity.normalized * MaxSpeed;
		}

		// Ajustar drag dinámicamente
		if (Input.GetKey(KeyCode.W))
		{
			rb.drag = normalDrag; // libre al acelerar
		}
		else
		{
			rb.drag = stopDrag; // frena en seco al soltar W
		}
	}

	private void HandleInput()
	{
		X = 0;
		Y = 0;

		// Acelerar con W
		if (Input.GetKey(KeyCode.W))
		{
			Y = 1;
		}
		// Retroceder con S (opcional)
		else if (Input.GetKey(KeyCode.S))
		{
			Y = -1;
		}

		// Girar con A/D (invertido según tu pedido anterior)
		if (Input.GetKey(KeyCode.A))
		{
			X = 1; // A = derecha
		}
		else if (Input.GetKey(KeyCode.D))
		{
			X = -1; // D = izquierda
		}
	}
}
