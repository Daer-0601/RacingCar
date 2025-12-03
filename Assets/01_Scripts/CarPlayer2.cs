using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarPlayer2 : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float reverseSpeed = 6f;  // Velocidad en reversa
    public float steering = 250f;

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Control de giro")]
    public float minSpeedToSteer = 0.1f;
    public float steeringSensitivity = 1.0f; // Sensibilidad del giro
    public float angularDrag = 5f; // Fricción angular cuando no hay input

    [Header("Controles Jugador 2")]
    public KeyCode accelerateKey = KeyCode.UpArrow;
    public KeyCode brakeKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;

    private Rigidbody2D _rb;
    private float _steerInput = 0f;
    private float _accelInput = 0f; // 1 acelera -1 frena/retro
    private float _currentAngularDrag = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
        _rb.angularDrag = 0f; // Controlaremos el angular drag manualmente
    }

    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
        ApplyLateralFriction();
        LimitSpeed();
        UpdateDrag();
    }

    private void ReadInput()
    {
        _accelInput = 0f;
        _steerInput = 0f;

        // Aceleración / Retroceso Jugador 2
        if (Input.GetKey(accelerateKey))
            _accelInput = 1f;
        else if (Input.GetKey(brakeKey))
            _accelInput = -1f;

        // Dirección Jugador 2 - leer independientemente
        if (Input.GetKey(leftKey))
            _steerInput = 1f;  // Invertido: izquierda = rotación positiva
        if (Input.GetKey(rightKey))
            _steerInput = -1f; // Invertido: derecha = rotación negativa
        
        // Si ambas teclas están presionadas, no girar (o puedes elegir una prioridad)
        if (Input.GetKey(leftKey) && Input.GetKey(rightKey))
            _steerInput = 0f;
    }

    private void ApplyEngineForce()
    {
        // Acelerar
        if (_accelInput > 0f)
        {
            _rb.AddForce(transform.up * acceleration, ForceMode2D.Force);
        }
        // Freno / Retroceso
        else if (_accelInput < 0f)
        {
            _rb.AddForce(transform.up * (-reverseSpeed), ForceMode2D.Force);
        }
    }

    private void ApplySteering()
    {
        float speed = _rb.velocity.magnitude;
        bool hasSteerInput = Mathf.Abs(_steerInput) > 0.01f;

        // Si no hay input de giro, aplicar fricción angular para detener el giro
        if (!hasSteerInput)
        {
            // Aplicar fricción angular para detener el giro gradualmente
            _currentAngularDrag = Mathf.Lerp(_currentAngularDrag, angularDrag, Time.fixedDeltaTime * 5f);
            _rb.angularDrag = _currentAngularDrag;
            
            // Si la velocidad angular es muy baja, detenerla completamente
            if (Mathf.Abs(_rb.angularVelocity) < 0.5f)
            {
                _rb.angularVelocity = 0f;
            }
            
            return;
        }

        // Resetear angular drag cuando hay input
        _rb.angularDrag = 0f;
        _currentAngularDrag = 0f;

        // Evitar giros cuando está completamente quieto
        if (speed < 0.05f)
        {
            _rb.angularVelocity = 0f;
            return;
        }

        // Aplicar giro con sensibilidad ajustable
        if (speed > minSpeedToSteer)
        {
            // Factor de velocidad: más velocidad = más capacidad de giro
            float speedFactor = Mathf.Clamp01(speed / maxSpeed);
            // Asegurar un mínimo de giro incluso a baja velocidad
            speedFactor = Mathf.Max(speedFactor, 0.4f);
            
            // Calcular el giro con sensibilidad
            float steerAmount = _steerInput * steering * speedFactor * steeringSensitivity * Time.fixedDeltaTime;
            
            // Aplicar el giro de forma suave
            _rb.rotation += steerAmount;
        }
        // Si está moviéndose pero muy lento, permitir giro reducido
        else if (speed > 0.05f)
        {
            // Giro mínimo a muy baja velocidad
            float steerAmount = _steerInput * steering * 0.5f * steeringSensitivity * Time.fixedDeltaTime;
            _rb.rotation += steerAmount;
        }
    }

    private void ApplyLateralFriction()
    {
        Vector2 lateralVel = Vector2.Dot(_rb.velocity, transform.right) * transform.right;
        _rb.AddForce(-lateralVel * lateralFriction, ForceMode2D.Force);
    }

    private void LimitSpeed()
    {
        if (_rb.velocity.magnitude > maxSpeed)
            _rb.velocity = _rb.velocity.normalized * maxSpeed;
    }

    private void UpdateDrag()
    {
        float targetDrag = (_accelInput != 0f) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, targetDrag, Time.fixedDeltaTime * 4f);
    }
}
