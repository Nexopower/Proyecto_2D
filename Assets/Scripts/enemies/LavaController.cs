using UnityEngine;

public class LavaController : MonoBehaviour
{
    [Header("Lava Settings")]
    public float riseHeight = 3f;
    public float riseSpeed = 2f;
    public float riseInterval = 4f;
    public float stayUpDuration = 2f;
    public bool startActive = false;
    public int damageAmount = 2;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private Vector3 startPosition;
    private Vector3 baseScale;
    private Vector3 targetScale;
    private bool isRising = false;
    private bool isUp = false;
    private bool isLowering = false;
    private float timer = 0f;
    private Animator animator;
    private Collider2D col;

    void Start()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        startPosition = transform.position;
        baseScale = transform.localScale;
        
        // Calcular la escala objetivo basada en la altura deseada
        float scaleMultiplier = 1f + (riseHeight / transform.localScale.y);
        targetScale = new Vector3(baseScale.x, baseScale.y * scaleMultiplier, baseScale.z);
        
        // Configurar el punto de anclaje para que escale desde abajo
        // Esto hace que la base se mantenga fija y solo se estire hacia arriba
        SetPivotToBottom();
        
        // Inicializar estado
        if (startActive)
        {
            transform.localScale = targetScale;
            AdjustPositionForBottomPivot(transform.localScale);
            isUp = true;
            timer = stayUpDuration;
            col.enabled = true;
            if (showDebugLogs)
                Debug.Log("Lava - Started in UP scale");
        }
        else
        {
            transform.localScale = baseScale;
            AdjustPositionForBottomPivot(transform.localScale);
            col.enabled = false;
            timer = riseInterval;
            if (showDebugLogs)
                Debug.Log("Lava - Started in DOWN scale");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        if (!isRising && !isUp && !isLowering && timer <= 0f)
        {
            // Estado de espera terminado, empezar a subir
            StartRising();
        }
        else if (isRising)
        {
            // Proceso de estiramiento hacia arriba
            float step = riseSpeed * Time.deltaTime;
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, step);
            
            // Ajustar posición para mantener la base fija
            AdjustPositionForBottomPivot(transform.localScale);
            
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                // Llegó a la escala máxima
                transform.localScale = targetScale;
                AdjustPositionForBottomPivot(transform.localScale);
                isRising = false;
                isUp = true;
                timer = stayUpDuration;
                col.enabled = true;
                
                if (showDebugLogs)
                    Debug.Log("Lava - Reached UP scale, staying for " + stayUpDuration + "s");
            }
        }
        else if (isUp && timer <= 0f)
        {
            // Tiempo arriba terminado, empezar a contraerse
            StartLowering();
        }
        else if (isLowering)
        {
            // Proceso de contracción hacia abajo
            float step = riseSpeed * Time.deltaTime;
            transform.localScale = Vector3.MoveTowards(transform.localScale, baseScale, step);
            
            // Ajustar posición para mantener la base fija
            AdjustPositionForBottomPivot(transform.localScale);
            
            if (Vector3.Distance(transform.localScale, baseScale) < 0.01f)
            {
                // Llegó a la escala base
                transform.localScale = baseScale;
                AdjustPositionForBottomPivot(transform.localScale);
                isLowering = false;
                timer = riseInterval;
                col.enabled = false;
                
                if (showDebugLogs)
                    Debug.Log("Lava - Reached DOWN scale, waiting for " + riseInterval + "s");
            }
        }
        

    }

    void StartRising()
    {
        isRising = true;
        isUp = false;
        isLowering = false;
        
        if (showDebugLogs)
            Debug.Log("Lava - Started RISING (scaling up)");
        
        // Activar animación de subida si existe
    }

    void StartLowering()
    {
        isRising = false;
        isUp = false;
        isLowering = true;
        
        if (showDebugLogs)
            Debug.Log("Lava - Started LOWERING (scaling down)");
        
        // Activar animación de bajada si existe

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (isUp || isRising))
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                Vector2 directiondamage = (collision.transform.position - transform.position).normalized;
                playerScript.GetDamage(directiondamage, damageAmount);
                
                if (showDebugLogs)
                    Debug.Log("Lava - Player damaged for " + damageAmount + " damage");
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 basePos = Application.isPlaying ? startPosition : transform.position;
        Vector3 topPos = basePos + Vector3.up * riseHeight;
        
        // Mostrar rango de estiramiento
        Gizmos.color = Color.red;
        Gizmos.DrawLine(basePos, topPos);
        
        // Mostrar posición base (siempre fija)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(basePos, Vector3.one * 0.5f);
        
        // Mostrar altura máxima que alcanzará
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(topPos, Vector3.one * 0.5f);
        
        // Mostrar estado actual en tiempo de ejecución
        if (Application.isPlaying)
        {
            // Mostrar área de daño cuando está activa (basado en escala actual)
            if (isUp || isRising)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                if (GetComponent<Collider2D>() != null)
                {
                    Vector3 currentSize = Vector3.Scale(GetComponent<Collider2D>().bounds.size, transform.localScale);
                    Gizmos.DrawCube(transform.position, currentSize);
                }
            }
            
            // Mostrar estado con indicadores
            Gizmos.color = Color.white;
            Vector3 labelPos = transform.position + Vector3.up * (transform.localScale.y * 0.5f + 0.5f);
            
            if (isRising)
                Gizmos.DrawWireSphere(labelPos, 0.1f); // Círculo pequeño para "estirando"
            else if (isUp)
                Gizmos.DrawSphere(labelPos, 0.1f); // Círculo sólido para "estirado"
            else if (isLowering)
                Gizmos.DrawWireCube(labelPos, Vector3.one * 0.2f); // Cubo para "contrayendo"
            else
                Gizmos.DrawRay(labelPos, Vector3.down * 0.3f); // Rayo para "esperando"
        }
    }

    /// <summary>
    /// Configura el punto de anclaje para que el escalado ocurra desde la parte inferior
    /// </summary>
    void SetPivotToBottom()
    {
        // En Unity 2D, para que un objeto escale desde abajo, necesitamos ajustar su posición
        // cuando cambie la escala, manteniendo la parte inferior fija
        
        // Obtenemos el bounds del collider para calcular el offset
        if (col != null)
        {
            // Calculamos donde debería estar la posición para que la base se mantenga fija
            float originalBottom = transform.position.y - (col.bounds.size.y * 0.5f);
            
            // Guardamos esta información para usar en el escalado
            startPosition = new Vector3(transform.position.x, originalBottom + (baseScale.y * 0.5f), transform.position.z);
        }
    }

    /// <summary>
    /// Ajusta la posición del objeto cuando escala para mantener la base fija
    /// </summary>
    void AdjustPositionForBottomPivot(Vector3 currentScale)
    {
        if (col != null)
        {
            // Calculamos la nueva posición Y para mantener la base fija
            float scaledHeight = currentScale.y;
            float newPosY = startPosition.y - (baseScale.y * 0.5f) + (scaledHeight * 0.5f);
            
            transform.position = new Vector3(startPosition.x, newPosY, startPosition.z);
        }
    }
}
