using UnityEngine;

public class SlimeGreen : MonoBehaviour
{
    public float movementSpeed = 1f;
    public int hp = 2;
    public bool squished = false;
    public bool hit = false;
    public bool canBeSquished = true;
    public bool getdamage;
    public float fuerzarebote= 6f; // Fuerza del rebote al recibir daño
    public float squishThreshold = 0.5f; // Umbral para determinar si el jugador está encima
    public float squishDuration = 2f; // Duración del aplastamiento

    public float wallCheckDistance = 0.2f;
    public LayerMask Wall;
    
    private Animator animator;
    private Rigidbody2D rb;
    private bool playervivo=true;
    private float originalMovementSpeed; // Guardar velocidad original

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalMovementSpeed = movementSpeed; // Guardar velocidad original
    }

    void Update()
    {

        if (playervivo && !squished && hp > 0)
        {
            float direction = Mathf.Sign(transform.localScale.x);
            if (!getdamage)
                transform.Translate(Vector2.right * direction * movementSpeed * Time.deltaTime);

            if (EnemyUtils.DetectWall(transform, wallCheckDistance, Wall, Vector2.right * direction))
                EnemyUtils.Flip(transform);
        }
        
        if(!playervivo || squished || hp <= 0)
        {
            movementSpeed = 0;
        }

        animator.SetFloat("movement", movementSpeed);
        animator.SetBool("squished", squished);
        animator.SetBool("hit", getdamage);
        animator.SetInteger("hp", hp);
        hit = false;
    }

    // Método público para reactivar el movimiento (llamar desde evento de animación)
    public void RecoverFromSquish()
    {
        squished = false;
        canBeSquished = true;
        movementSpeed = originalMovementSpeed; // Restaurar velocidad
        
        Debug.Log("Slime recuperado y listo para moverse!");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canBeSquished && !squished)
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            // Verificar si el jugador está tocando la parte superior del slime
            bool playerAboveSlime = collision.transform.position.y > transform.position.y + 0.2f;
            
            if (playerAboveSlime)
            {
                // El jugador aplasta al slime
                SquishSlime();
                
                // Dar un pequeño rebote al jugador hacia arriba
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                playerRb.AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
                
                Debug.Log("Slime aplastado!");
            }
            else
            {
                // El jugador toca al slime por los lados o desde abajo
                Vector2 directiondamage = new Vector2(transform.position.x, 0);
                playerScript.GetDamage(directiondamage, 1);
                playervivo = playerScript.hp > 0;
                
                Debug.Log("Jugador recibe daño");
            }
        }
    }
    
    private void SquishSlime()
    {
        squished = true;
        canBeSquished = false;
        GetDamage(Vector2.zero, 1); // Hacer daño al slime
        
        // Opcional: agregar efecto visual o sonido aquí
    }
    
    public void OnTriggerEnter2D (Collider2D collision)
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
            hp -= cantdamage;
            
            if (hp > 0)
            {
                if (!squished) // Solo aplicar rebote si no está aplastado
                {
                    Vector2 rebote = new Vector2(transform.position.x - direction.x, 0.2f).normalized;
                    rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);
                }
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
        if (!squished && hp > 0) // Solo resetear velocidad si no está aplastado y tiene hp
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 dir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * wallCheckDistance);
        
        // Dibujar el área de detección para aplastar
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.up * squishThreshold, Vector3.one * 0.1f);
    }
}
