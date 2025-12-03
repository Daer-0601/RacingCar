using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float reverseSpeed = 6f;  // velocidad en reversa
    public float turboMultiplier = 1.7f; // turbo

    [Header("Giro")]
    public float steering = 250f;
    public float minSpeedToSteer = 0.1f;
    public float steeringDeadzone = 20f;
    public float steeringSensitivity = 1.5f;

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Controles")]
    public bool useKeyboardInput = true;

    private Rigidbody2D _rb;

    private float _steerInput = 0f;
    private float _accelInput = 0f; // 1 acelera -1 frena/retro
    private bool _turboActive = false;

    private bool _useArduinoInput = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
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

        // Evitar giros cuando está quieto
        if (_rb.velocity.magnitude < 0.05f)
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

        if (Mathf.Abs(angle) < steeringDeadzone)
        {
            _steerInput = 0f;
        }
        else
        {
            float adjusted = angle - steeringDeadzone * Mathf.Sign(angle);
            _steerInput = Mathf.Clamp(
                (adjusted / (450f - steeringDeadzone)) * steeringSensitivity,
                -1f,
                1f
            );
        }
    }

    // ---------------------------
    // ACELERADOR desde Arduino
    // ---------------------------
    public void SetAccelInput(int accel)
    {
        _useArduinoInput = true;
        _accelInput = accel == 1 ? 1f : (_accelInput > 0 ? 0f : _accelInput);
    }

    // ---------------------------
    // FRENO + RETROCESO desde Arduino
    // ---------------------------
    public void SetBrakeInput(int brake)
    {
        _useArduinoInput = true;
        _accelInput = brake == 1 ? -1f : (_accelInput < 0 ? 0f : _accelInput);
    }

    // ---------------------------
    // TURBO desde Arduino
    // ---------------------------
    public void SetTurboInput(int turbo)
    {
        _useArduinoInput = true;
        _turboActive = (turbo == 1);
    }

    // ---------------------------
    // MOTOR
    // ---------------------------
    private void ApplyEngineForce()
    {
        float finalAccel = _accelInput;

        // Acelerar
        if (_accelInput > 0f)
        {
            float accelValue = acceleration;

            if (_turboActive)
                accelValue *= turboMultiplier;

            _rb.AddForce(transform.up * accelValue, ForceMode2D.Force);
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

        if (speed > minSpeedToSteer && Mathf.Abs(_steerInput) > 0.01f)
        {
            float speedFactor = Mathf.Clamp01(speed / maxSpeed);
            float steerAmount = _steerInput * steering * speedFactor * Time.fixedDeltaTime;
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
        float currentMax = _turboActive ? maxSpeed * turboMultiplier : maxSpeed;

        if (_rb.velocity.magnitude > currentMax)
            _rb.velocity = _rb.velocity.normalized * currentMax;
    }

    private void UpdateDrag()
    {
        float target = (_accelInput != 0f) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, target, Time.fixedDeltaTime * 4f);
    }
}
