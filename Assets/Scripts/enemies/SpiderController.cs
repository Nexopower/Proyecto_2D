using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("Spider Settings")]
    public int hp = 2;
    public bool hit = false;
    public bool jump = false;
    public float movement = 1f;
    public float movementSpeed = 2f;
    public float chaseSpeed = 4f;
    public float jumpForce = 8f;
    public float detectionRange = 3f;
    public float jumpCooldown = 2f; // Tiempo que tarda para volver a saltar
    public float wallCheckDistance = 0.2f;
    public float edgeCheckDistance = 0.5f;
    public float groundCheckDistance = 0.1f; // Distancia del raycast para detectar suelo
    public bool canBeSquished = true;
    public bool stopAtEdge = true;
    public float fuerzarebote = 6f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private bool getdamage = false;
    private bool playervivo = true;
    private bool isChasing = false;
    private bool canJump = true;
    private bool isGrounded = true;
    private float lastJumpTime = 0f;
    private float originalMovementSpeed;
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalMovementSpeed = movementSpeed;
    }

    void Update()
    {
        CheckGrounded();
        
        if (playervivo && hp > 0 && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool playerInRange = distanceToPlayer <= detectionRange;
            bool playerInFront = EnemyUtils.IsPlayerInFront(transform, player);
            
            // Detectar si el jugador está en rango ENFRENTE (no en la espalda)
            if (playerInRange && playerInFront)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    if (showDebugLogs)
                        Debug.Log("Spider - Started chasing player");
                }
                
                // Aumentar velocidad cuando está persiguiendo
                movement = chaseSpeed;
                
                // Saltar hacia el jugador si puede (cooldown terminado)
                if (canJump && isGrounded && Time.time - lastJumpTime >= jumpCooldown)
                {
                    JumpTowardsPlayer();
                    if (showDebugLogs)
                        Debug.Log("Spider - Jumped towards player");
                }
                else if (!canJump && showDebugLogs)
                {
                    Debug.Log($"Spider - Can't jump yet, cooldown: {jumpCooldown - (Time.time - lastJumpTime):F1}s");
                }
            }
            else
            {
                // Jugador fuera de rango o detrás, comportamiento normal
                if (isChasing)
                {
                    isChasing = false;
                    movement = originalMovementSpeed;
                    if (showDebugLogs)
                        Debug.Log("Spider - Stopped chasing player");
                }
            }

            // Movimiento horizontal (como un goomba)
            if (isGrounded && !getdamage)
            {
                float direction = Mathf.Sign(transform.localScale.x);
                
                // Verificar pared
                if (EnemyUtils.DetectWall(transform, wallCheckDistance, wallLayer, Vector2.right * direction))
                {
                    EnemyUtils.Flip(transform);
                }
                // Verificar borde si está configurado para detenerse
                else if (stopAtEdge && EnemyUtils.DetectEdge(transform, edgeCheckDistance, groundLayer, Vector2.right * direction))
                {
                    EnemyUtils.Flip(transform);
                }
                // Verificar colisión con otros enemigos
                else if (EnemyUtils.DetectEnemy(transform, wallCheckDistance, enemyLayer, Vector2.right * direction))
                {
                    EnemyUtils.Flip(transform);
                }
                else
                {
                    // Moverse horizontalmente
                    transform.Translate(Vector2.right * direction * movement * Time.deltaTime);
                }
            }
        }
        else
        {
            movement = 0;
            isChasing = false;
        }

        // Actualizar cooldown de salto
        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            canJump = true;
        }

        // Actualizar parámetros del animator
        animator.SetBool("hit", hit);
        animator.SetBool("jump", jump);
        animator.SetFloat("movement", movement);
        animator.SetInteger("hp", hp);
        
        hit = false;
        jump = false;
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    void JumpTowardsPlayer()
    {
        if (player == null) return;
        
        canJump = false;
        jump = true;
        lastJumpTime = Time.time;
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 jumpVector = new Vector2(directionToPlayer.x * jumpForce * 0.7f, jumpForce);
        
        rb.AddForce(jumpVector, ForceMode2D.Impulse);
        
        // Voltear hacia el jugador
        if (directionToPlayer.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (directionToPlayer.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeSquished)
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            bool playerAbove = collision.transform.position.y > transform.position.y + 0.2f;
            
            if (playerAbove && canBeSquished)
            {
                GetDamage(Vector2.zero, 1);
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                playerRb.AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
            }
            else
            {
                Vector2 directiondamage = new Vector2(transform.position.x, 0);
                playerScript.GetDamage(directiondamage, 1);
                playervivo = playerScript.hp > 0;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon"))
        {
            Vector2 directiondamage = new Vector2(collision.gameObject.transform.position.x, 0);
            GetDamage(directiondamage, 1);
        }
    }

    public void GetDamage(Vector2 direction, int cantdamage)
    {
        if (!getdamage)
        {
            getdamage = true;
            hit = true;
            hp -= cantdamage;
            
            if (hp > 0 && direction != Vector2.zero)
            {
                Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);
            }
        }
    }

    public void destruir()
    {
        Destroy(gameObject);
    }

    public void ResetDamage()
    {
        getdamage = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void OnDrawGizmos()
    {
        // Rango de detección (solo hacia adelante)
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Vector3 detectionCenter = transform.position + forward * (detectionRange / 2);
        Gizmos.DrawWireCube(detectionCenter, new Vector3(detectionRange, detectionRange * 0.5f, 0));
        
        // Detección de pared
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + forward * wallCheckDistance);
        
        // Detección de borde
        Gizmos.color = Color.blue;
        Vector3 edgeCheckPos = transform.position + forward * 0.5f;
        Gizmos.DrawLine(edgeCheckPos, edgeCheckPos + Vector3.down * edgeCheckDistance);
        
        // Detección de suelo (ground check)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        
        // Línea hacia el jugador cuando está en rango
        if (isChasing && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
        
        // Indicador de cooldown de salto
        if (Application.isPlaying && !canJump)
        {
            Gizmos.color = Color.orange;
            float cooldownProgress = (Time.time - lastJumpTime) / jumpCooldown;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.2f * cooldownProgress);
        }
        
        // Indicador de estado
        if (Application.isPlaying)
        {
            Gizmos.color = isChasing ? Color.red : Color.white;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, 0.1f);
        }
    }
}
