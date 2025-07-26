using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Ghost Settings")]
    public int hp = 2;
    public bool hit = false;
    public bool tracking = false;
    public float movementSpeed = 2f;
    public float chaseSpeed = 2f;
    public float detectionRange = Mathf.Infinity; // Rango infinito para detectar al jugador sin importar la distancia
    public bool canBeSquished = false;
    public float fuerzarebote = 6f;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private bool getdamage = false;
    private bool playervivo = true;
    private float originalMovementSpeed;
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 lastPlayerDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalMovementSpeed = movementSpeed;
        
        // Configurar el fantasma para que pueda atravesar paredes y suelos
        rb.bodyType = RigidbodyType2D.Kinematic; // No se ve afectado por la física
        GetComponent<Collider2D>().isTrigger = true; // Puede atravesar objetos
    }

    void Update()
    {
        if (playervivo && hp > 0 && player != null)
        {
            // El fantasma siempre puede detectar al jugador (rango infinito)
            // Verificar si el jugador le está dando la espalda
            bool playerLookingAtGhost = IsPlayerLookingAtGhost();
            
            if (showDebugLogs)
            {
                Debug.Log($"Ghost - Player looking at ghost: {playerLookingAtGhost}");
            }
            
            if (!playerLookingAtGhost)
            {
                // El jugador le está dando la espalda, perseguir
                tracking = true;
                    movementSpeed = chaseSpeed;
                    
                    // Moverse hacia el jugador
                    if (!getdamage)
                    {
                        Vector2 direction = (player.position - transform.position).normalized;
                        transform.Translate(direction * movementSpeed * Time.deltaTime);
                        
                        // Voltear el fantasma hacia el jugador
                        if (direction.x > 0)
                            transform.localScale = new Vector3(1, 1, 1);
                        else if (direction.x < 0)
                            transform.localScale = new Vector3(-1, 1, 1);
                        
                        lastPlayerDirection = direction;
                }
            }
            else
            {
                // El jugador lo está mirando, detenerse
                tracking = false;
                movementSpeed = 0;
                
                if (showDebugLogs)
                {
                    Debug.Log("Ghost - Player is looking, stopping");
                }
            }
        }
        else
        {
            tracking = false;
            movementSpeed = 0;
        }        // Actualizar parámetros del animator
        animator.SetBool("traking", tracking);
        animator.SetBool("hit", hit);
        animator.SetInteger("hp", hp);
        
        hit = false;
    }

    bool IsPlayerLookingAtGhost()
    {
        if (player == null) return false;
        
        // Obtener la dirección hacia donde mira el jugador basándose en su localScale
        Vector2 playerLookDirection = player.localScale.x > 0 ? Vector2.right : Vector2.left;
        
        // Obtener la dirección del jugador hacia el fantasma
        Vector2 directionToGhost = (transform.position - player.position).normalized;
        
        // Calcular el producto punto para ver si están mirando en direcciones similares
        float dotProduct = Vector2.Dot(playerLookDirection, directionToGhost);
        
        // Si el producto punto es positivo, el jugador está mirando hacia el fantasma
        return dotProduct > 0.3f; // Umbral para dar un poco de tolerancia
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                Vector2 directiondamage = (collision.transform.position - transform.position).normalized;
                playerScript.GetDamage(directiondamage, 1);
                playervivo = playerScript.hp > 0;
                
                if (showDebugLogs)
                {
                    Debug.Log("Ghost - Player damaged");
                }
            }
        }
        else if (collision.CompareTag("weapon"))
        {
            Vector2 directiondamage = (collision.transform.position - transform.position).normalized;
            GetDamage(directiondamage, 1);
            
            if (showDebugLogs)
            {
                Debug.Log("Ghost - Weapon hit");
            }
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
        // No resetear velocidad porque el fantasma no usa físicas
    }

    void OnDrawGizmos()
    {
 // Círculo visual grande para mostrar que el rango es infinito
        
        // Línea hacia el jugador cuando está rastreando
        if (tracking && player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
        
        // Indicador de dirección del fantasma
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, forward * 1f);
        
        // Mostrar si el jugador está mirando al fantasma
        if (Application.isPlaying && player != null)
        {
            bool playerLooking = IsPlayerLookingAtGhost();
            Gizmos.color = playerLooking ? Color.green : Color.magenta;
            Vector3 playerLookDir = player.localScale.x > 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(player.position, playerLookDir * 2f);
        }
    }
}
