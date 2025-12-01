using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarPlayer2 : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;
    public float acceleration = 8f;
    public float steering = 250f;

    [Header("Fricción")]
    public float normalDrag = 0.5f;
    public float stopDrag = 4f;
    public float lateralFriction = 2f;

    [Header("Control de giro")]
    public float minSpeedToSteer = 0.1f;

    [Header("Controles Jugador 2")]
    public KeyCode accelerateKey = KeyCode.UpArrow;
    public KeyCode brakeKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;

    private Rigidbody2D _rb;
    private float _steerInput;
    private float _accelInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
    }

    void Update()
    {
        ReadInput();

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

    private void ReadInput()
    {
        _accelInput = 0f;
        _steerInput = 0f;

        // Aceleración Jugador 2
        if (Input.GetKey(accelerateKey))
            _accelInput = 1f;
        else if (Input.GetKey(brakeKey))
            _accelInput = -1f;

        // Dirección Jugador 2
        if (Input.GetKey(leftKey))
            _steerInput = -1f;
        else if (Input.GetKey(rightKey))
            _steerInput = 1f;
    }

    private void ApplyEngineForce()
    {
        if (_accelInput != 0f)
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
        float targetDrag = (_accelInput != 0f) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, targetDrag, Time.fixedDeltaTime * 5f);
    }
}