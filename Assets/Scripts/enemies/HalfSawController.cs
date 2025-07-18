using UnityEngine;

public class HalfSawController : MonoBehaviour
{
    public enum MovementType
    {
        FloorHorizontal,
        WallVertical,
        AroundObject,
        AroundSquare
    }
    [Header("Half Saw Settings")]
    
    public MovementType movementType = MovementType.FloorHorizontal;
    public float movementSpeed = 2f;
    public float movementRange = 3f;
    public bool clockwise = true;
    public Transform objectToOrbit; // Para el movimiento alrededor de un objeto
    public float orbitRadius = 2f;
    public float squareWidth = 2f; // Ancho del rectángulo para AroundSquare
    public float squareHeight = 2f; // Alto del rectángulo para AroundSquare
    public bool rotateOnSquare = true; // Si debe rotar en cada lado del cuadrado
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    
    private Vector3 startPosition;
    private float currentDistance = 0f;
    private float currentAngle = 0f;
    private int direction = 1;
    private Animator animator;
    private int currentSide = 0; // Para el movimiento en cuadrado (0=derecha, 1=arriba, 2=izquierda, 3=abajo)
    private float sideProgress = 0f; // Progreso en el lado actual del cuadrado

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        
        // Si es movimiento alrededor de objeto y no hay objeto asignado, usar la posición actual como centro
        if ((movementType == MovementType.AroundObject || movementType == MovementType.AroundSquare) && objectToOrbit == null)
        {
            GameObject centerObject = new GameObject("OrbitCenter");
            centerObject.transform.position = transform.position;
            objectToOrbit = centerObject.transform;
        }
    }

    void Update()
    {
        switch (movementType)
        {
            case MovementType.FloorHorizontal:
                MoveFloorHorizontal();
                break;
            case MovementType.WallVertical:
                MoveWallVertical();
                break;
            case MovementType.AroundObject:
                MoveAroundObject();
                break;
            case MovementType.AroundSquare:
                MoveAroundSquare();
                break;
        }
    }

    void MoveFloorHorizontal()
    {
        currentDistance += movementSpeed * Time.deltaTime * direction;
        
        // Verificar si debe cambiar de dirección por rango o por borde
        Vector3 nextPosition = new Vector3(startPosition.x + currentDistance, startPosition.y, transform.position.z);
        
        // Verificar suelo tanto arriba como abajo
        RaycastHit2D groundCheckDown = Physics2D.Raycast(nextPosition, Vector2.down, 0.5f, groundLayer);
        RaycastHit2D groundCheckUp = Physics2D.Raycast(nextPosition, Vector2.up, 0.5f, groundLayer);
        
        bool hasGround = groundCheckDown.collider != null || groundCheckUp.collider != null;
        
        if (Mathf.Abs(currentDistance) >= movementRange || !hasGround)
        {
            direction *= -1;
            currentDistance = Mathf.Clamp(currentDistance, -movementRange, movementRange);
        }
        
        transform.position = new Vector3(startPosition.x + currentDistance, startPosition.y, transform.position.z);
    }

    void MoveWallVertical()
    {
        currentDistance += movementSpeed * Time.deltaTime * direction;
        
        // Verificar si debe cambiar de dirección por rango o por pérdida de pared
        Vector3 nextPosition = new Vector3(startPosition.x, startPosition.y + currentDistance, transform.position.z);
        
        // Verificar pared tanto a la derecha como a la izquierda
        RaycastHit2D wallCheckRight = Physics2D.Raycast(nextPosition, Vector2.right, 0.5f, wallLayer);
        RaycastHit2D wallCheckLeft = Physics2D.Raycast(nextPosition, Vector2.left, 0.5f, wallLayer);
        
        bool hasWall = wallCheckRight.collider != null || wallCheckLeft.collider != null;
        
        if (Mathf.Abs(currentDistance) >= movementRange || !hasWall)
        {
            direction *= -1;
            currentDistance = Mathf.Clamp(currentDistance, -movementRange, movementRange);
        }
        
        transform.position = new Vector3(startPosition.x, startPosition.y + currentDistance, transform.position.z);
    }

    void MoveAroundObject()
    {
        if (objectToOrbit == null) return;
        
        currentAngle += movementSpeed * Time.deltaTime * (clockwise ? 1 : -1);
        
        float x = objectToOrbit.position.x + Mathf.Cos(currentAngle) * orbitRadius;
        float y = objectToOrbit.position.y + Mathf.Sin(currentAngle) * orbitRadius;
        
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void MoveAroundSquare()
    {
        if (objectToOrbit == null) return;
        
        // Avanzar en el lado actual
        sideProgress += movementSpeed * Time.deltaTime;
        
        // Obtener la longitud del lado actual
        float currentSideLength = (currentSide % 2 == 0) ? squareHeight : squareWidth;
        
        // Si completó un lado, pasar al siguiente
        if (sideProgress >= currentSideLength)
        {
            sideProgress = 0f;
            if (clockwise)
                currentSide = (currentSide + 1) % 4;
            else
                currentSide = (currentSide - 1 + 4) % 4;
        }
        
        // Calcular posición basada en el lado actual
        Vector3 center = objectToOrbit.position;
        Vector3 newPosition = center;
        float halfWidth = squareWidth * 0.5f;
        float halfHeight = squareHeight * 0.5f;
        
        switch (currentSide)
        {
            case 0: // Lado derecho (moviéndose hacia arriba)
                newPosition = center + new Vector3(halfWidth, -halfHeight + sideProgress, 0);
                break;
            case 1: // Lado superior (moviéndose hacia izquierda)
                newPosition = center + new Vector3(halfWidth - sideProgress, halfHeight, 0);
                break;
            case 2: // Lado izquierdo (moviéndose hacia abajo)
                newPosition = center + new Vector3(-halfWidth, halfHeight - sideProgress, 0);
                break;
            case 3: // Lado inferior (moviéndose hacia derecha)
                newPosition = center + new Vector3(-halfWidth + sideProgress, -halfHeight, 0);
                break;
        }
        
        transform.position = newPosition;
        
        // Aplicar rotación según el lado si está habilitado
        if (rotateOnSquare)
        {
            float targetRotation = 0f;
            switch (currentSide)
            {
                case 1: // Lado superior (físicamente) - rotación 0°
                    targetRotation = 0f; // Arriba = 0°
                    break;
                case 3: // Lado inferior (físicamente) - rotación 180°
                    targetRotation = 180f; // Abajo = 180°
                    break;
                case 2: // Lado izquierdo (físicamente) - rotación 90°
                    targetRotation = 90f; // Izquierda = 90°
                    break;
                case 0: // Lado derecho (físicamente) - rotación 270°
                    targetRotation = 270f; // Derecha = 270°
                    break;
            }
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
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
            case MovementType.FloorHorizontal:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(center + Vector3.left * movementRange, center + Vector3.right * movementRange);
                // Mostrar detección de suelo en ambas direcciones
                Gizmos.color = Color.green;
                Gizmos.DrawRay(center + Vector3.left * movementRange, Vector3.down * 0.5f);
                Gizmos.DrawRay(center + Vector3.right * movementRange, Vector3.down * 0.5f);
                Gizmos.DrawRay(center + Vector3.left * movementRange, Vector3.up * 0.5f);
                Gizmos.DrawRay(center + Vector3.right * movementRange, Vector3.up * 0.5f);
                break;
                
            case MovementType.WallVertical:
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(center + Vector3.down * movementRange, center + Vector3.up * movementRange);
                // Mostrar detección de pared en ambas direcciones
                Gizmos.color = Color.red;
                Gizmos.DrawRay(center + Vector3.down * movementRange, Vector3.right * 0.5f);
                Gizmos.DrawRay(center + Vector3.up * movementRange, Vector3.right * 0.5f);
                Gizmos.DrawRay(center + Vector3.down * movementRange, Vector3.left * 0.5f);
                Gizmos.DrawRay(center + Vector3.up * movementRange, Vector3.left * 0.5f);
                break;
                
            case MovementType.AroundObject:
                if (objectToOrbit != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(objectToOrbit.position, orbitRadius);
                }
                break;
                
            case MovementType.AroundSquare:
                if (objectToOrbit != null)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 squareCenter = objectToOrbit.position;
                    float halfWidth = squareWidth * 0.5f;
                    float halfHeight = squareHeight * 0.5f;
                    
                    // Dibujar el rectángulo
                    Vector3[] corners = new Vector3[4]
                    {
                        squareCenter + new Vector3(halfWidth, halfHeight, 0),    // Superior derecha
                        squareCenter + new Vector3(-halfWidth, halfHeight, 0),   // Superior izquierda
                        squareCenter + new Vector3(-halfWidth, -halfHeight, 0),  // Inferior izquierda
                        squareCenter + new Vector3(halfWidth, -halfHeight, 0)    // Inferior derecha
                    };
                    
                    // Dibujar las líneas del rectángulo
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
                    }
                    
                    // Mostrar lado actual y rotación en tiempo de ejecución
                    if (Application.isPlaying)
                    {
                        Gizmos.color = Color.red;
                        Vector3 currentPos = transform.position;
                        Gizmos.DrawWireSphere(currentPos, 0.1f);
                        
                        // Mostrar dirección de rotación
                        if (rotateOnSquare)
                        {
                            Gizmos.color = Color.cyan;
                            Vector3 forward = transform.up; // El "up" del objeto rotado
                            Gizmos.DrawRay(currentPos, forward * 0.5f);
                        }
                    }
                }
                break;
        }
    }
}
