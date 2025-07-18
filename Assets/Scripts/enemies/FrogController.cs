using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Frog Settings")]
    public int hp = 3;
    public bool hit = false;
    public bool suelo = false;
    public float jumpForceHorizontal = 5f;
    public float jumpForceVertical = 8f;
    public float jumpInterval = 2f;
    public float chaseJumpForceHorizontal = 8f;
    public float chaseJumpForceVertical = 12f;
    public float chaseJumpInterval = 0.8f;
    public float detectionRange = 4f;
    public float wallCheckDistance = 0.2f;
    public float groundCheckDistance = 0.1f;
    public float groundCheckWidth = 0.1f;
    public float offsetRayoX = 0f;
    public float offsetRayoY = 0f;
    public bool canBeSquished = true;
    public bool stopAtEdge = true;
    public float fuerzarebote = 6f;
    public LayerMask wallLayer;
    public LayerMask enemyLayer;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private bool getdamage = false;
    private bool playervivo = true;
    private bool isChasing = false;
    private float jumpTimer = 0f;
    private float currentJumpInterval;
    private float currentJumpForceHorizontal;
    private float currentJumpForceVertical;
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Inicializar valores de salto
        currentJumpInterval = jumpInterval;
        currentJumpForceHorizontal = jumpForceHorizontal;
        currentJumpForceVertical = jumpForceVertical;
        jumpTimer = jumpInterval; // Empezar con el timer listo para saltar
    }

    void Update()
    {
        CheckGrounded();
        
        if (playervivo && hp > 0 && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool playerInRange = distanceToPlayer <= detectionRange;
            
            // Detectar si debe perseguir al jugador
            if (playerInRange && !isChasing)
            {
                isChasing = true;
                currentJumpInterval = chaseJumpInterval;
                currentJumpForceHorizontal = chaseJumpForceHorizontal;
                currentJumpForceVertical = chaseJumpForceVertical;
                
                // Girar hacia el jugador cuando empiece a perseguirlo
                Vector2 directionToPlayer = player.position - transform.position;
                if (directionToPlayer.x > 0 && transform.localScale.x < 0)
                {
                    EnemyUtils.Flip(transform);
                }
                else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
                {
                    EnemyUtils.Flip(transform);
                }
            }
            else if (!playerInRange && isChasing)
            {
                isChasing = false;
                currentJumpInterval = jumpInterval;
                currentJumpForceHorizontal = jumpForceHorizontal;
                currentJumpForceVertical = jumpForceVertical;
            }

            // Sistema de salto
            if (suelo && !getdamage)
            {
                jumpTimer -= Time.deltaTime;
                
                if (jumpTimer <= 0f)
                {
                    if (showDebugLogs)
                    {
                        Debug.Log("Frog - Realizando salto");
                    }
                    PerformJump();
                    jumpTimer = currentJumpInterval;
                }
            }
        }

        // Actualizar parámetros del animator
        animator.SetBool("suelo", suelo);
        animator.SetBool("hit", hit);
        animator.SetInteger("hp", hp);
        
        hit = false;
    }

    void PerformJump()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 jumpDirection = Vector2.right * direction;
        
        // Si está persiguiendo al jugador, asegurarse de que mire hacia él antes de saltar
        if (isChasing && player != null)
        {
            Vector2 directionToPlayer = player.position - transform.position;
            if (directionToPlayer.x > 0 && transform.localScale.x < 0)
            {
                EnemyUtils.Flip(transform);
            }
            else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
            {
                EnemyUtils.Flip(transform);
            }
            // Actualizar la dirección después del posible giro
            direction = Mathf.Sign(transform.localScale.x);
            jumpDirection = Vector2.right * direction;
        }
        
        // Verificar pared
        if (EnemyUtils.DetectWall(transform, wallCheckDistance, wallLayer, jumpDirection))
        {
            EnemyUtils.Flip(transform);
            direction = Mathf.Sign(transform.localScale.x);
            jumpDirection = Vector2.right * direction;
        }
        // Verificar borde si está configurado para detenerse
        else if (stopAtEdge && DetectEdge(jumpDirection))
        {
            EnemyUtils.Flip(transform);
            direction = Mathf.Sign(transform.localScale.x);
            jumpDirection = Vector2.right * direction;
        }
        // Verificar colisión con otros enemigos
        else if (EnemyUtils.DetectEnemy(transform, wallCheckDistance, enemyLayer, jumpDirection))
        {
            EnemyUtils.Flip(transform);
            direction = Mathf.Sign(transform.localScale.x);
            jumpDirection = Vector2.right * direction;
        }
        
        // Calcular la fuerza del salto separadamente
        Vector2 jumpForce = Vector2.zero;
        
        // Si está persiguiendo al jugador, saltar hacia él
        if (isChasing && player != null)
        {
            Vector2 playerDirection = (player.position - transform.position).normalized;
            jumpForce = new Vector2(playerDirection.x * currentJumpForceHorizontal, currentJumpForceVertical);
        }
        else
        {
            jumpForce = new Vector2(jumpDirection.x * currentJumpForceHorizontal, currentJumpForceVertical);
        }
        
        // Aplicar el salto
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Resetear velocidad vertical
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }

    bool DetectEdge(Vector2 direction)
    {
        // Usar la misma lógica que el jugador para detectar el suelo
        Vector3 edgeCheckPos = transform.position + new Vector3(direction.x * 0.5f, 0, 0);
        Vector3 rayPos = edgeCheckPos + new Vector3(offsetRayoX, offsetRayoY - 0.5f, 0);
        RaycastHit2D hit = Physics2D.BoxCast(rayPos, new Vector2(groundCheckWidth, 0.05f), 0f, Vector2.down, groundCheckDistance);
        
        // Retorna true si NO hay suelo (es un borde) o si solo detecta a sí mismo
        return hit.collider == null || hit.collider.gameObject == gameObject;
    }

    void CheckGrounded()
    {
        // Usar BoxCast desde fuera del collider de la rana
        Vector3 rayPos = transform.position + new Vector3(offsetRayoX, offsetRayoY - 0.5f, 0);
        RaycastHit2D hit = Physics2D.BoxCast(rayPos, new Vector2(groundCheckWidth, 0.05f), 0f, Vector2.down, groundCheckDistance);
        
        bool wasGrounded = suelo;
        
        // Verificar si el hit no es el propio collider de la rana
        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            suelo = true;
        }
        else
        {
            suelo = false;
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"Frog - Suelo: {suelo}, Hit: {hit.collider?.name}, Self: {hit.collider?.gameObject == gameObject}, Velocity Y: {rb.linearVelocity.y}");
        }
        
        // Si acaba de tocar el suelo después de estar en el aire, detener la velocidad
        if (suelo && !wasGrounded && rb.linearVelocity.y <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            if (showDebugLogs)
            {
                Debug.Log("Frog - Velocidad reseteada al tocar suelo");
            }
        }
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
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Detección de pared
        Gizmos.color = Color.red;
        Vector3 dir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * wallCheckDistance);
        
        // Detección de suelo (similar al jugador)
        Gizmos.color = Color.blue;
        Vector3 centro = transform.position + new Vector3(offsetRayoX, offsetRayoY - 0.5f, 0);
        Vector3 tamaño = new Vector3(groundCheckWidth, groundCheckDistance, 0);
        Gizmos.DrawWireCube(centro + Vector3.down * (groundCheckDistance / 2), tamaño);
        
        // Detección de borde
        Gizmos.color = Color.green;
        Vector3 edgeCheckPos = transform.position + dir * 0.5f;
        Vector3 edgeCentro = edgeCheckPos + new Vector3(offsetRayoX, offsetRayoY - 0.5f, 0);
        Vector3 edgeTamaño = new Vector3(groundCheckWidth, groundCheckDistance, 0);
        Gizmos.DrawWireCube(edgeCentro + Vector3.down * (groundCheckDistance / 2), edgeTamaño);
    }
}
