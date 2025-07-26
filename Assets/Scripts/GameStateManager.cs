using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("Configuración del Juego")]
    public int vidasMaximas = 3;
    public int metaMonedas = 10; // Número de monedas para ganar
    public bool ganarPorTiempo = false;
    public float tiempoLimite = 60f;
    
    [Header("Referencias")]
    public GameManager gameManager;
    
    private bool juegoTerminado = false;
    private float tiempoInicio;
    
    void Start()
    {
        // Si no hay referencia al GameManager, buscar uno en la escena
        if (gameManager == null)
            gameManager = GameManager.Instance;
        
        tiempoInicio = Time.time;
        
        if (gameManager == null)
        {
            Debug.LogWarning("GameStateManager: No se encontró GameManager en la escena");
        }
    }
    
    void Update()
    {
        if (juegoTerminado || gameManager == null) return;
        
        // Verificar condiciones de victoria
        if (VerificarVictoria())
        {
            juegoTerminado = true;
            IrAVictoria();
        }
        // Verificar condiciones de derrota
        else if (VerificarDerrota())
        {
            juegoTerminado = true;
            IrADerrota();
        }
    }
    
    bool VerificarVictoria()
    {
        // Victoria por monedas recolectadas
        if (gameManager.score >= metaMonedas)
        {
            Debug.Log($"¡Victoria! Monedas recolectadas: {gameManager.score}");
            return true;
        }
        
        // Victoria por tiempo (si está habilitado)
        if (ganarPorTiempo && (Time.time - tiempoInicio) >= tiempoLimite)
        {
            Debug.Log("¡Victoria por tiempo!");
            return true;
        }
        
        return false;
    }
    
    bool VerificarDerrota()
    {
        // Buscar al jugador para verificar su vida
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && player.hp <= 0)
        {
            Debug.Log("¡Derrota! El jugador se quedó sin vida");
            return true;
        }
        
        return false;
    }
    
    void IrAVictoria()
    {
        Debug.Log("Transición a pantalla de victoria...");
        // Esperar un poco antes de cambiar de escena
        Invoke(nameof(CargarEscenaVictoria), 1.5f);
    }
    
    void IrADerrota()
    {
        Debug.Log("Transición a pantalla de derrota...");
        // Esperar un poco antes de cambiar de escena
        Invoke(nameof(CargarEscenaDerrota), 1.5f);
    }
    
    void CargarEscenaVictoria()
    {
        GameManager.GoToWinScene();
    }
    
    void CargarEscenaDerrota()
    {
        GameManager.GoToLoseScene();
    }
    
    // Métodos públicos para llamar desde otros scripts
    public void ForzarVictoria()
    {
        if (!juegoTerminado)
        {
            juegoTerminado = true;
            IrAVictoria();
        }
    }
    
    public void ForzarDerrota()
    {
        if (!juegoTerminado)
        {
            juegoTerminado = true;
            IrADerrota();
        }
    }
    
    // Getters para información del juego
    public float TiempoTranscurrido()
    {
        return Time.time - tiempoInicio;
    }
    
    public float TiempoRestante()
    {
        if (ganarPorTiempo)
            return Mathf.Max(0, tiempoLimite - TiempoTranscurrido());
        return 0;
    }
    
    public bool JuegoTerminado()
    {
        return juegoTerminado;
    }
}
