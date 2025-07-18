using UnityEngine;

public class SawController : MonoBehaviour
{
    public enum MovementType
    {
        Circular,
        Horizontal,
        Vertical
    }
    [Header("Saw Settings")]
    
    public MovementType movementType = MovementType.Circular;
    public float movementSpeed = 2f;
    public float movementRange = 3f;
    public bool clockwise = true;
    
    private Vector3 startPosition;
    private float currentAngle = 0f;
    private float currentDistance = 0f;
    private int direction = 1;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        switch (movementType)
        {
            case MovementType.Circular:
                MoveCircular();
                break;
            case MovementType.Horizontal:
                MoveHorizontal();
                break;
            case MovementType.Vertical:
                MoveVertical();
                break;
        }
        
        // Rotar la sierra
        transform.Rotate(0, 0, movementSpeed * 100 * Time.deltaTime * (clockwise ? 1 : -1));
    }

    void MoveCircular()
    {
        currentAngle += movementSpeed * Time.deltaTime * (clockwise ? 1 : -1);
        
        float x = startPosition.x + Mathf.Cos(currentAngle) * movementRange;
        float y = startPosition.y + Mathf.Sin(currentAngle) * movementRange;
        
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void MoveHorizontal()
    {
        currentDistance += movementSpeed * Time.deltaTime * direction;
        
        if (Mathf.Abs(currentDistance) >= movementRange)
        {
            direction *= -1;
            currentDistance = Mathf.Clamp(currentDistance, -movementRange, movementRange);
        }
        
        transform.position = new Vector3(startPosition.x + currentDistance, startPosition.y, transform.position.z);
    }

    void MoveVertical()
    {
        currentDistance += movementSpeed * Time.deltaTime * direction;
        
        if (Mathf.Abs(currentDistance) >= movementRange)
        {
            direction *= -1;
            currentDistance = Mathf.Clamp(currentDistance, -movementRange, movementRange);
        }
        
        transform.position = new Vector3(startPosition.x, startPosition.y + currentDistance, transform.position.z);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                Vector2 directiondamage = new Vector2(transform.position.x, 0);
                playerScript.GetDamage(directiondamage, 1);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        
        switch (movementType)
        {
            case MovementType.Circular:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(center, movementRange);
                break;
                
            case MovementType.Horizontal:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(center + Vector3.left * movementRange, center + Vector3.right * movementRange);
                break;
                
            case MovementType.Vertical:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(center + Vector3.down * movementRange, center + Vector3.up * movementRange);
                break;
        }
    }
}
