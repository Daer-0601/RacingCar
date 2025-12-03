using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarBot : MonoBehaviour
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

    [Header("IA del Bot")]
    public Transform target; // Objetivo a seguir (puede ser el jugador o un waypoint)
    public float detectionDistance = 10f;
    public float avoidanceDistance = 3f;
    public float turnDetectionDistance = 5f; // Distancia para detectar tags de giro
    public LayerMask obstacleLayer = -1;

    [Header("Anti-Atascamiento")]
    public bool enableUnstuck = true; // Activar/desactivar sistema anti-atascamiento
    public float stuckCheckDistance = 0.3f; // Distancia mínima de movimiento
    public float stuckTimeThreshold = 3f; // Tiempo antes de desatascarse (aumentado)
    public float unstuckForce = 3f; // Fuerza para desatascarse (reducida)

    private Rigidbody2D _rb;
    private float _steerInput;
    private float _accelInput;
    private Vector2 _lastPosition;
    private float _stuckTimer = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = normalDrag;
        _rb.gravityScale = 0f;
    }

    void Start()
    {
        // Si no hay target asignado, buscar al jugador
        if (target == null)
        {
            FindPlayerTarget();
        }
        
        _lastPosition = transform.position;
    }

    private void FindPlayerTarget()
    {
        // Buscar por tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        // Si hay múltiples objetos con tag "Player", buscar el que no sea este bot
        if (player != null && player == gameObject)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in allPlayers)
            {
                if (p != gameObject && p.GetComponent<CarMovement>() != null)
                {
                    player = p;
                    break;
                }
            }
        }
        
        if (player != null)
        {
            target = player.transform;
            Debug.Log("Bot: Target encontrado - " + player.name);
        }
        else
        {
            Debug.LogWarning("Bot: No se encontró un target (jugador). El bot no funcionará correctamente.");
        }
    }

    void Update()
    {
        CalculateBotInput();
        
        if (enableUnstuck)
        {
            CheckIfStuck();
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

    private bool CheckTurnTags()
    {
        // Raycast hacia adelante para detectar tags de giro
        Vector2 forward = (Vector2)transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, turnDetectionDistance);

        if (hit.collider != null)
        {
            // Detectar tag "TurnLeft" - girar a la izquierda
            if (hit.collider.CompareTag("TurnLeft"))
            {
                _steerInput = -1f;
                return true;
            }
            
            // Detectar tag "TurnRight" - girar a la derecha
            if (hit.collider.CompareTag("TurnRight"))
            {
                _steerInput = 1f;
                return true;
            }
        }
        
        return false;
    }

    private void CalculateBotInput()
    {
        // Inicializar valores por defecto
        _accelInput = 1f; // Por defecto, siempre acelerar
        _steerInput = 0f; // Resetear giro

        if (target == null)
        {
            // Si no hay target, al menos intentar avanzar
            _accelInput = 1f;
            _steerInput = 0f;
            return;
        }

        // Verificar tags de giro primero
        bool tagDetected = CheckTurnTags();
        
        // Si no hay tag de giro detectado, seguir al objetivo normal
        if (!tagDetected)
        {
            // Calcular dirección hacia el objetivo
            Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

            // Seguir al objetivo
            _steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        }

        // Detección de obstáculos (ignorar waypoints y turn objects)
        Vector2 forward = (Vector2)transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, avoidanceDistance, obstacleLayer);

        // Solo evitar si es un obstáculo real (no waypoint ni turn)
        if (hit.collider != null && !IsNavigationObject(hit.collider) && !hit.collider.CompareTag("Player"))
        {
            // Evitar obstáculo solo si está muy cerca
            if (hit.distance < avoidanceDistance * 0.7f)
            {
                Vector2 right = (Vector2)transform.right;
                Vector2 leftDir = (forward - right * 0.5f).normalized;
                Vector2 rightDir = (forward + right * 0.5f).normalized;
                
                RaycastHit2D leftCheck = Physics2D.Raycast(transform.position, leftDir, avoidanceDistance * 0.7f, obstacleLayer);
                RaycastHit2D rightCheck = Physics2D.Raycast(transform.position, rightDir, avoidanceDistance * 0.7f, obstacleLayer);
                
                // Ignorar objetos de navegación en los checks laterales
                if (leftCheck.collider != null && IsNavigationObject(leftCheck.collider))
                    leftCheck = new RaycastHit2D();
                if (rightCheck.collider != null && IsNavigationObject(rightCheck.collider))
                    rightCheck = new RaycastHit2D();
                
                if (leftCheck.collider == null && rightCheck.collider != null)
                    _steerInput = -1f; // Girar izquierda
                else if (rightCheck.collider == null && leftCheck.collider != null)
                    _steerInput = 1f; // Girar derecha
                else
                    _steerInput *= 0.5f; // Reducir giro si hay obstáculo al frente
                
                _accelInput = 0.7f; // Reducir velocidad ligeramente
            }
        }

        // Ajustar aceleración según distancia al objetivo
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget < 2f)
        {
            _accelInput = Mathf.Max(_accelInput, 0.5f); // Mantener velocidad mínima
        }
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
        _rb.drag = Mathf.Lerp(_rb.drag, targetDrag, Time.fixedDeltaTime * 5f);
    }

    // Verificar si un collider es un objeto de navegación (waypoint o turn)
    private bool IsNavigationObject(Collider2D collider)
    {
        if (collider == null) return false;
        
        return collider.CompareTag("Waypoint") || 
               collider.CompareTag("TurnLeft") || 
               collider.CompareTag("TurnRight");
    }

    // Verificar si el bot está atascado y aplicar fuerza para desatascarse
    private void CheckIfStuck()
    {
        float distanceMoved = Vector2.Distance(transform.position, _lastPosition);
        float speed = _rb.velocity.magnitude;
        
        // Solo considerar atascado si no se mueve Y tiene velocidad muy baja
        if (distanceMoved < stuckCheckDistance && speed < 0.5f)
        {
            _stuckTimer += Time.deltaTime;
            
            // Si está atascado por mucho tiempo, aplicar fuerza suave para desatascarse
            if (_stuckTimer > stuckTimeThreshold)
            {
                Unstuck();
                _stuckTimer = 0f;
            }
        }
        else
        {
            // Se está moviendo, resetear timer
            _stuckTimer = 0f;
        }
        
        _lastPosition = transform.position;
    }

    // Aplicar fuerza suave para desatascarse
    private void Unstuck()
    {
        // Aplicar fuerza hacia atrás suave
        Vector2 backward = -(Vector2)transform.up;
        _rb.AddForce(backward * unstuckForce, ForceMode2D.Force);
        
        // Girar suavemente en la dirección del objetivo si existe
        if (target != null)
        {
            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float angleToTarget = Vector2.SignedAngle(transform.up, toTarget);
            _rb.rotation += Mathf.Sign(angleToTarget) * 15f; // Giro suave de 15 grados
        }
        else
        {
            // Si no hay target, girar aleatoriamente pero suave
            float randomRotation = Random.Range(-15f, 15f);
            _rb.rotation += randomRotation;
        }
    }
}

