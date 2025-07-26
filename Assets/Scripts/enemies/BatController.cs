using System.Collections;
using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Bat Settings")]
    public int hp = 2;
    public bool hit = false;
    public bool player = false; // Se activa cuando detecta al jugador
    public bool isHanging = true; // Variable para animación idle colgando
    public float detectionRange = 4f;
    public float flySpeed = 3f;
    public float returnSpeed = 2f;
    public float fuerzarebote = 3f;
    
    private bool getdamage = false;
    private bool playervivo = true;
    private bool isFlying = false;
    private bool isReturning = false;
    private Vector3 originalPosition;
    private Transform playerTransform;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalPosition = transform.position;
        
        // Inicialmente está colgando
        isHanging = true;
        isFlying = false;
        player = false;
    }

    void Update()
    {
        if (playervivo && hp > 0 && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool playerInRange = distanceToPlayer <= detectionRange;
            
            if (playerInRange && !isFlying && !isReturning)
            {
                // Empezar a volar y perseguir
                StartFlying();
            }
            else if (!playerInRange && isFlying && !isReturning)
            {
                // Empezar a regresar
                StartReturning();
            }
            
            // Movimiento
            if (!getdamage)
            {
                if (isFlying && !isReturning)
                {
                    // Perseguir al jugador
                    Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                    transform.Translate(directionToPlayer * flySpeed * Time.deltaTime);
                    
                    // Voltear hacia el jugador
                    if (directionToPlayer.x > 0)
                        transform.localScale = new Vector3(1, 1, 1);
                    else if (directionToPlayer.x < 0)
                        transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (isReturning)
                {
                    // Regresar a la posición original
                    Vector2 directionToHome = (originalPosition - transform.position).normalized;
                    transform.Translate(directionToHome * returnSpeed * Time.deltaTime);
                    
                    // Voltear hacia el punto de origen
                    if (directionToHome.x > 0)
                        transform.localScale = new Vector3(1, 1, 1);
                    else if (directionToHome.x < 0)
                        transform.localScale = new Vector3(-1, 1, 1);
                    
                    // Verificar si ha llegado a la posición original
                    if (Vector2.Distance(transform.position, originalPosition) < 0.1f)
                    {
                        transform.position = originalPosition;
                        isReturning = false;
                        isFlying = false;
                        isHanging = true;
                        player = false;
                    }
                }
            }
        }
        else
        {
            // Si el jugador está muerto, regresar a la posición original
            if (isFlying && !isReturning)
            {
                StartReturning();
            }
        }

        // Actualizar parámetros del animator
        animator.SetBool("hit", hit);
        animator.SetBool("player", player);
        animator.SetBool("isHanging", isHanging);
        animator.SetInteger("hp", hp);
        
        hit = false;
    }

    void StartFlying()
    {
        isFlying = true;
        isHanging = false;
        player = true;
        Debug.Log("Murciélago empezando a volar!");
    }

    void StartReturning()
    {
        isReturning = true;
        player = false;
        Debug.Log("Murciélago regresando a casa!");
    }

    void BounceAwayFromPlayer(Vector3 playerPosition)
    {
        // Calcular la dirección opuesta al jugador
        Vector2 bounceDirection = (transform.position - playerPosition).normalized;

        // Añadir un poco de fuerza vertical para que se vea más natural
        bounceDirection.y = Mathf.Max(bounceDirection.y, 0.3f);

        // Aplicar la fuerza de rebote
        rb.AddForce(bounceDirection * 40, ForceMode2D.Impulse);
        
    }

    IEnumerator ResetAfterBounce(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        rb.linearVelocity = Vector2.zero;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BounceAwayFromPlayer(collision.gameObject.transform.position);
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            Vector2 directiondamage = new Vector2(transform.position.x, 0);
            playerScript.GetDamage(directiondamage, 1);
            playervivo = playerScript.hp > 0;

            StartCoroutine(ResetAfterBounce(0.5f));
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
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmos()
    {
        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Posición original
        Vector3 homePos = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(homePos, Vector3.one * 0.3f);
        
        // Línea hacia el jugador cuando está volando
        if (isFlying && !isReturning && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
        
        // Línea hacia casa cuando está regresando
        if (isReturning)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, homePos);
        }
        
        // Indicador de estado
        if (Application.isPlaying)
        {
            if (isHanging)
                Gizmos.color = Color.gray;
            else if (isFlying && !isReturning)
                Gizmos.color = Color.red;
            else if (isReturning)
                Gizmos.color = Color.blue;
            
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}
