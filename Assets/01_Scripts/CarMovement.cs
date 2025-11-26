using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 12f;       // Velocidad máxima
    public float acceleration = 8f;    // Fuerza de aceleración
    public float steering = 180f;      // Velocidad de giro

    [Header("Fricción")]
    public float normalDrag = 0.5f;    // Resistencia al acelerar
    public float stopDrag = 4f;        // Resistencia al soltar acelerador
    public float lateralFriction = 2f; // Fuerza contra el deslizamiento lateral

    private Rigidbody2D _rb;
    private float _steerInput;
    private float _accelInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0; // aseguramos que no caiga
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
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

        if (Input.GetKey(KeyCode.W)) _accelInput = 1f;
        else if (Input.GetKey(KeyCode.S)) _accelInput = -1f;

        if (Input.GetKey(KeyCode.A)) _steerInput = 1f;
        else if (Input.GetKey(KeyCode.D)) _steerInput = -1f;
    }

    private void ApplyEngineForce()
    {
        Vector2 force = transform.up * (_accelInput * acceleration);
        _rb.AddForce(force, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        if (Mathf.Abs(_accelInput) > 0.1f)
        {
            float speedFactor = _rb.velocity.magnitude / maxSpeed;
            _rb.rotation += _steerInput * steering * speedFactor * Time.fixedDeltaTime;
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
        {
            _rb.velocity = _rb.velocity.normalized * maxSpeed;
        }
    }

    private void UpdateDrag()
    {
        float targetDrag = (_accelInput > 0) ? normalDrag : stopDrag;
        _rb.drag = Mathf.Lerp(_rb.drag, targetDrag, Time.fixedDeltaTime * 5f);
    }
}
