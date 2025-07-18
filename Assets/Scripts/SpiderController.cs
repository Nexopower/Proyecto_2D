using UnityEngine;

public class Spider : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float chaseSpeed = 2f;
    public float jumpForce = 5f;
    public float detectionRange = 3f;
    public float jumpCooldown = 2f;
    public int hp = 2;
    public bool canBeSquished = true;
    public bool hit = false;
    public bool jump = false;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastJumpTime;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        float speed = movementSpeed;

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool playerInRange = EnemyUtils.IsPlayerInRange(transform, player, detectionRange) && EnemyUtils.IsPlayerInFront(transform, player);

        if (playerInRange)
        {
            speed = chaseSpeed;
            if (grounded && Time.time - lastJumpTime > jumpCooldown)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                lastJumpTime = Time.time;
                jump = true;
            }
        }

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (EnemyUtils.DetectWall(transform, wallCheckDistance, wallLayer, Vector2.right * direction))
            EnemyUtils.Flip(transform);

        animator.SetFloat("movement", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("hit", hit);
        animator.SetBool("jump", jump);
        hit = false;
        jump = false;
    }

    public void TakeDamage(int damage, bool isSquished)
    {
        if (isSquished && !canBeSquished) return;

        hp -= damage;
        hit = true;

        if (hp <= 0)
            Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        Gizmos.color = Color.red;
        Vector3 dir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * wallCheckDistance);
    }
}
