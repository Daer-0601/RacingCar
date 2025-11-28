using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float steering = 250f; // aumentado para más sensibilidad

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Control de giro")]
    public float minSpeedToSteer = 0.1f;
    public float steeringDeadzone = 20f; // reducido para mayor respuesta
    public float steeringSensitivity = 1.5f; // nuevo parámetro ajustable

    [Header("Controles")]
    public bool useKeyboardInput = true;

    private Rigidbody2D _rb;
    private float _steerInput;
    private float _accelInput;
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
            _accelInput = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ? 1f : 0f;

            _steerInput = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _steerInput = -1f;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _steerInput = 1f;
        }

        if (_rb.velocity.magnitude < 0.05f)
        {
            _steerInput = 0f;
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
    // Giro desde Arduino
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
            float adjustedAngle = angle - (steeringDeadzone * Mathf.Sign(angle));
            _steerInput = Mathf.Clamp(
                (adjustedAngle / (300f - steeringDeadzone)) * steeringSensitivity,
                -1f,
                1f
            );
        }
    }

    // ---------------------------
    // Acelerador desde Arduino
    // ---------------------------
    public void SetAccelInput(int accel)
    {
        _useArduinoInput = true;
        _accelInput = Mathf.Clamp01(accel);
    }

    private void ApplyEngineForce()
    {
        if (_accelInput > 0f)
        {
            Vector2 force = transform.up * (_accelInput * acceleration);
            _rb.AddForce(force, ForceMode2D.Force);
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
        else if (speed < 0.05f)
        {
            _rb.angularVelocity = 0f;
        }
    }

    private void ApplyLateralFriction()
    {
        Vector2 lateralVel = Vector2.Dot(_rb.velocity, transform.right) * (Vector2)transform.right;
        _rb.AddForce(-lateralVel * lateralFriction, ForceMode2D.Force);
    }

    private void LimitSpeed()
    {
        if (_rb.velocity.magnitude > maxSpeed)
            _rb.velocity = _rb.velocity.normalized * maxSpeed;
    }

    private void UpdateDrag()
    {
        float targetDrag = (_accelInput > 0.01f) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, targetDrag, Time.fixedDeltaTime * 5f);
    }
}
