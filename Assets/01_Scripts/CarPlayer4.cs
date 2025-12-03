using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarPlayer4 : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float reverseSpeed = 6f;
    public float steering = 250f;

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Control de giro")]
    public float minSpeedToSteer = 0.1f;
    public float steeringSensitivity = 1.0f;
    public float angularDrag = 5f;

    [Header("Controles Jugador 4 (Numpad 8-4-5-6)")]
    public KeyCode accelerateKey = KeyCode.Keypad8;
    public KeyCode brakeKey = KeyCode.Keypad5;
    public KeyCode leftKey = KeyCode.Keypad4;
    public KeyCode rightKey = KeyCode.Keypad6;

    private Rigidbody2D _rb;
    private float _steerInput = 0f;
    private float _accelInput = 0f;
    private float _currentAngularDrag = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
        _rb.angularDrag = 0f;
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

        // Aceleración / Retroceso
        if (Input.GetKey(accelerateKey))
            _accelInput = 1f;
        else if (Input.GetKey(brakeKey))
            _accelInput = -1f;

        // Dirección
        if (Input.GetKey(leftKey))
            _steerInput = 1f;
        if (Input.GetKey(rightKey))
            _steerInput = -1f;
        
        if (Input.GetKey(leftKey) && Input.GetKey(rightKey))
            _steerInput = 0f;
    }

    private void ApplyEngineForce()
    {
        if (_accelInput > 0f)
        {
            _rb.AddForce(transform.up * acceleration, ForceMode2D.Force);
        }
        else if (_accelInput < 0f)
        {
            _rb.AddForce(transform.up * (-reverseSpeed), ForceMode2D.Force);
        }
    }

    private void ApplySteering()
    {
        float speed = _rb.velocity.magnitude;
        bool hasSteerInput = Mathf.Abs(_steerInput) > 0.01f;

        if (!hasSteerInput)
        {
            _currentAngularDrag = Mathf.Lerp(_currentAngularDrag, angularDrag, Time.fixedDeltaTime * 5f);
            _rb.angularDrag = _currentAngularDrag;
            
            if (Mathf.Abs(_rb.angularVelocity) < 0.5f)
            {
                _rb.angularVelocity = 0f;
            }
            
            return;
        }

        _rb.angularDrag = 0f;
        _currentAngularDrag = 0f;

        if (speed < 0.05f)
        {
            _rb.angularVelocity = 0f;
            return;
        }

        if (speed > minSpeedToSteer)
        {
            float speedFactor = Mathf.Clamp01(speed / maxSpeed);
            speedFactor = Mathf.Max(speedFactor, 0.4f);
            
            float steerAmount = _steerInput * steering * speedFactor * steeringSensitivity * Time.fixedDeltaTime;
            _rb.rotation += steerAmount;
        }
        else if (speed > 0.05f)
        {
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

