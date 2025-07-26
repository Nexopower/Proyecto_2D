using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Block Settings (Thwomp)")]
    public bool falling = false;
    public float detectionRange = 4f;
    public float fallSpeed = 8f;
    public float riseSpeed = 2f;
    public float waitTime = 2f;
    public float detectionWidth = 0.88f;
    public float groundCheckDistance = 0.39f;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private enum BlockState
    {
        Idle,
        Detecting,
        Falling,
        Waiting,
        Rising
    }
    
    private BlockState currentState = BlockState.Idle;
    private Vector3 startPosition;
    private bool hasHitGround = false;
    private float timer = 0f;
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPosition = transform.position;
        
        // Asegurar que el bloque esté estático inicialmente
        rb.bodyType = RigidbodyType2D.Static;
    }

    void Update()
    {
        switch (currentState)
        {
            case BlockState.Idle:
                CheckForPlayer();
                break;
                
            case BlockState.Detecting:
                CheckForPlayer();
                break;
                
            case BlockState.Falling:
                CheckGroundHit();
                break;
                
            case BlockState.Waiting:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    StartRising();
                }
                break;
                
            case BlockState.Rising:
                RiseToStart();
                break;
        }
        
        // Actualizar parámetros del animator
        if (animator != null)
        {
            animator.SetBool("falling", falling);
        }
    }

    void CheckForPlayer()
    {
        if (player == null) return;
        
        // Verificar si el jugador está debajo del bloque en el rango de detección
        Vector2 playerDirection = player.position - transform.position;
        
        bool playerBelow = playerDirection.y < 0 && Mathf.Abs(playerDirection.y) <= detectionRange;
        bool playerInWidth = Mathf.Abs(playerDirection.x) <= detectionWidth;
        
        if (playerBelow && playerInWidth && currentState != BlockState.Detecting)
        {
            currentState = BlockState.Detecting;
            if (showDebugLogs) Debug.Log("¡Thwomp detectó al jugador!");
            // Pequeña pausa antes de caer
            Invoke("StartFalling", 0.3f);
        }
        else if ((!playerBelow || !playerInWidth) && currentState == BlockState.Detecting)
        {
            currentState = BlockState.Idle;
            CancelInvoke("StartFalling");
            if (showDebugLogs) Debug.Log("Thwomp perdió al jugador");
        }
    }

    void StartFalling()
    {
        if (currentState != BlockState.Detecting) return;
        
        currentState = BlockState.Falling;
        falling = true;
        hasHitGround = false;
        
        if (showDebugLogs) Debug.Log("¡Thwomp comenzó a caer!");
        
        // Cambiar a dinámico y aplicar velocidad de caída
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.down * fallSpeed;
    }

    void CheckGroundHit()
    {
        if (hasHitGround) return;
        
        // Verificar si ha tocado el suelo con un raycast configurable
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
        if (hit.collider != null)
        {
            hasHitGround = true;
            falling = false;
            currentState = BlockState.Waiting;
            timer = waitTime;
            
            // Detener movimiento
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            
            // Efecto de temblor o partículas aquí si lo deseas
            if (showDebugLogs) Debug.Log("¡Thwomp ha golpeado el suelo!");
        }
    }

    void StartRising()
    {
        currentState = BlockState.Rising;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (showDebugLogs) Debug.Log("¡Thwomp comenzó a subir!");
    }

    void RiseToStart()
    {
        // Usar el rigidbody para el movimiento controlado
        Vector2 direction = (startPosition - transform.position).normalized;
        rb.linearVelocity = direction * riseSpeed;
        
        // Verificar si ha llegado a la posición inicial
        if (Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            transform.position = startPosition;
            currentState = BlockState.Idle;
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector2.zero;
            falling = false;
            if (showDebugLogs) Debug.Log("¡Thwomp regresó a su posición inicial!");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentState == BlockState.Falling)
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                // Calcular dirección correcta para el empuje
                Vector2 directiondamage = (collision.transform.position - transform.position).normalized;
                playerScript.GetDamage(directiondamage, 2); // Mucho daño
                if (showDebugLogs) Debug.Log("¡Thwomp golpeó al jugador!");
            }
        }
    }

    // Agregar método para detección de colisión también
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && currentState == BlockState.Falling)
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                // Calcular dirección correcta para el empuje
                Vector2 directiondamage = (collision.transform.position - transform.position).normalized;
                playerScript.GetDamage(directiondamage, 2); // Mucho daño
                if (showDebugLogs) Debug.Log("¡Thwomp golpeó al jugador con colisión!");
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 pos = Application.isPlaying ? startPosition : transform.position;
        
        // Mostrar rango de detección
        Gizmos.color = Color.yellow;
        Vector3 detectionCenter = pos + Vector3.down * (detectionRange / 2);
        Gizmos.DrawWireCube(detectionCenter, new Vector3(detectionWidth * 2, detectionRange, 0));
        
        // Mostrar posición inicial
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos, Vector3.one * 0.5f);
        
        // Mostrar línea de caída hasta el suelo o hasta la distancia máxima
        Gizmos.color = Color.red;
        RaycastHit2D groundHit = Physics2D.Raycast(pos, Vector2.down, 10f, groundLayer);
        if (groundHit.collider != null)
        {
            Gizmos.DrawLine(pos, groundHit.point);
        }
        else
        {
            Gizmos.DrawLine(pos, pos + Vector3.down * 10f);
        }
        
        // Mostrar raycast de detección de suelo (línea roja más gruesa)
        Gizmos.color = Color.red;
        Vector3 currentPos = Application.isPlaying ? transform.position : pos;
        Vector3 raycastEnd = currentPos + Vector3.down * groundCheckDistance;
        
        // Dibujar línea principal del raycast
        Gizmos.DrawLine(currentPos, raycastEnd);
        
        // Dibujar pequeñas líneas perpendiculares para mostrar el área de detección
        Vector3 perpendicular = Vector3.right * 0.1f;
        Gizmos.DrawLine(raycastEnd - perpendicular, raycastEnd + perpendicular);
        
        // Mostrar estado actual
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Vector3 textPos = transform.position + Vector3.up * 2f;
            // Unity no puede mostrar texto en Gizmos, pero podemos cambiar el color según el estado
            switch (currentState)
            {
                case BlockState.Idle:
                    Gizmos.color = Color.green;
                    break;
                case BlockState.Detecting:
                    Gizmos.color = Color.yellow;
                    break;
                case BlockState.Falling:
                    Gizmos.color = Color.red;
                    break;
                case BlockState.Waiting:
                    Gizmos.color = Color.blue;
                    break;
                case BlockState.Rising:
                    Gizmos.color = Color.magenta;
                    break;
            }
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
        }
    }
}
