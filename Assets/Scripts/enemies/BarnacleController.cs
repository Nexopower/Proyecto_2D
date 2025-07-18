using UnityEngine;

public class BarnacleController : MonoBehaviour
{
    [Header("Barnacle Settings")]
    public int hp = 2;
    public bool hit = false;
    private bool canBeSquished = false;
    public float fuerzarebote = 6f;
    private float detectionRange = 2f;
    private float attackRange = 1f;
    private float attackCooldown = 2f;
    public LayerMask playerLayer;

    private bool getdamage = false;
    private bool playervivo = true;
    private float lastAttackTime = 0f;
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (playervivo && hp > 0 && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool playerInRange = distanceToPlayer <= detectionRange;
            bool playerInAttackRange = distanceToPlayer <= attackRange;
            
            // Atacar si el jugador está en rango y ha pasado el cooldown
            if (playerInAttackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }

        // Actualizar parámetros del animator
        animator.SetBool("hit", hit);
        animator.SetInteger("hp", hp);
        
        hit = false;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        // Aquí puedes agregar lógica de ataque específica
        Debug.Log("Barnacle atacando!");
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
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        // // Rango de detección
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // // Rango de ataque
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
