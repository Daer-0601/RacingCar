using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float reverseSpeed = 6f;  // velocidad en reversa

    [Header("Giro")]
    [Tooltip("Velocidad base de giro del auto")]
    public float steering = 250f;
    [Tooltip("Velocidad mínima necesaria para poder girar")]
    public float minSpeedToSteer = 0.1f;
    [Tooltip("Zona muerta del volante (grados) para evitar drift")]
    public float steeringDeadzone = 20f;
    [Tooltip("Sensibilidad del giro del volante (1.0 = normal, mayor = más sensible, menor = menos sensible)")]
    [Range(0.1f, 5.0f)]
    public float steeringSensitivity = 1.5f;
    [Tooltip("Fricción angular cuando no hay input de giro (ayuda a estabilizar el auto)")]
    public float angularDrag = 5f;

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Controles")]
    public bool useKeyboardInput = true;

    private Rigidbody2D _rb;

    private float _steerInput = 0f;
    private float _accelInput = 0f; // 1 acelera -1 frena/retro
    private float _currentAngularDrag = 0f;

    private bool _useArduinoInput = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
        _rb.angularDrag = 0f; // Controlaremos el angular drag manualmente
    }

    void Update()
    {
        if (!_useArduinoInput && useKeyboardInput)
        {
            // Acelerar / Retroceder
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _accelInput = 1f;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _accelInput = -1f;
            else
                _accelInput = 0f;

            // Dirección
            _steerInput = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _steerInput = -1f;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _steerInput = 1f;
        }

        // Evitar giros cuando está quieto (solo para teclado, Arduino maneja su propio input)
        if (!_useArduinoInput && _rb.velocity.magnitude < 0.05f)
        {
            _steerInput = 0f;
            _rb.angularVelocity = 0f;
        }
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
        ApplyLateralFriction();
        LimitSpeed();
        UpdateDrag();
    }

    // ---------------------------
    // VOLANTE desde Arduino
    // ---------------------------
    public void SetSteerInput(int angle)
    {
        _useArduinoInput = true;

        // Aplicar zona muerta
        if (Mathf.Abs(angle) < steeringDeadzone)
        {
            _steerInput = 0f;
        }
        else
        {
            // Calcular el input ajustado (quitando la zona muerta)
            float adjusted = angle - steeringDeadzone * Mathf.Sign(angle);
            
            // Normalizar el rango (asumiendo que el volante va de -450 a +450 grados)
            // El rango efectivo después de quitar la zona muerta es: (450 - deadzone) en cada dirección
            float maxRange = 450f - steeringDeadzone;
            float normalizedInput = adjusted / maxRange;
            
            // Aplicar sensibilidad (multiplicador que afecta la respuesta del giro)
            // steeringSensitivity = 1.0 es normal, > 1.0 es más sensible, < 1.0 es menos sensible
            _steerInput = Mathf.Clamp(normalizedInput * steeringSensitivity, -1f, 1f);
        }
    }

    // ---------------------------
    // ACELERADOR desde Arduino
    // ---------------------------
    public void SetAccelInput(int accel)
    {
        _useArduinoInput = true;
        if (accel == 1)
        {
            _accelInput = 1f;
        }
        else if (_accelInput > 0f)
        {
            // Si estaba acelerando y ahora no, detener suavemente
            _accelInput = 0f;
        }
        // Si estaba frenando, mantener el freno
    }

    // ---------------------------
    // FRENO + RETROCESO desde Arduino
    // ---------------------------
    public void SetBrakeInput(int brake)
    {
        _useArduinoInput = true;
        if (brake == 1)
        {
            _accelInput = -1f;
        }
        else if (_accelInput < 0f)
        {
            // Si estaba frenando y ahora no, detener suavemente
            _accelInput = 0f;
        }
        // Si estaba acelerando, mantener la aceleración
    }

    // ---------------------------
    // MOTOR
    // ---------------------------
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
            
            // Calcular el giro con sensibilidad del volante
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
        float target = (_accelInput != 0f) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, target, Time.fixedDeltaTime * 4f);
    }
}
