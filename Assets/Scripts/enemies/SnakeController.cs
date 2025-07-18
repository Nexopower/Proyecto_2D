using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [Header("Snake Settings")]
    public int hp = 1;
    public bool hit = false;
    public float movement = 3f; // Más rápido que el slime
    public float movementSpeed = 3f;
    public float wallCheckDistance = 0.2f;
    public float edgeCheckDistance = 0.5f;
    public bool canBeSquished = true;
    public bool stopAtEdge = true;
    public float fuerzarebote = 6f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    
    private bool getdamage = false;
    private bool playervivo = true;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = movementSpeed;
    }

    void Update()
    {
        if (playervivo && hp > 0)
        {
            // Movimiento
            if (!getdamage)
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
                    transform.Translate(Vector2.right * direction * movement * Time.deltaTime);
                }
            }
        }
        else
        {
            movement = 0;
        }

        // Actualizar parámetros del animator
        animator.SetBool("hit", hit);
        animator.SetFloat("movement", movement);
        animator.SetInteger("hp", hp);
        
        hit = false;
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
        // Detección de pared
        Gizmos.color = Color.red;
        Vector3 dir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * wallCheckDistance);
        
        // Detección de borde
        Gizmos.color = Color.blue;
        Vector3 edgeCheckPos = transform.position + dir * 0.5f;
        Gizmos.DrawLine(edgeCheckPos, edgeCheckPos + Vector3.down * edgeCheckDistance);
        
        // Indicador de velocidad (línea más larga = más rápido)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + dir * (movement * 0.5f));
    }
}
