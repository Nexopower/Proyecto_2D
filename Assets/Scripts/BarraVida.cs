using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [Header("UI References")]
    public Image rellenoBarraVida; // Referencia a la imagen que representa la barra de vida
    public Text vidaTexto; // Opcional: texto que muestre HP actual/máximo
    
    [Header("Player Reference")]
    public PlayerController playerController; // Referencia al controlador del jugador
    
    [Header("Settings")]
    public float maxVida = 3f; // Vida máxima del jugador (configurable)
    public bool updateEveryFrame = true; // Si actualizar cada frame o solo cuando cambie
    
    private float vidaAnterior; // Para detectar cambios
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Buscar el player si no está asignado
        if (playerController == null)
        {
            FindPlayerController();
        }
        
        // Configurar vida máxima
        if (playerController != null)
        {
            maxVida = playerController.hp; // Usar la vida actual como máxima al inicio
            vidaAnterior = playerController.hp;
        }
        
        // Actualizar barra inicialmente
        ActualizarBarraVida();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null)
        {
            FindPlayerController();
            return;
        }

        // Actualizar solo si ha cambiado la vida o si está configurado para actualizar siempre
        if (updateEveryFrame || playerController.hp != vidaAnterior)
        {
            ActualizarBarraVida();
            vidaAnterior = playerController.hp;
        }
    }
    
    private void FindPlayerController()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        
        // Si aún no encuentra el player, intentar usar Find como respaldo
        if (playerController == null)
        {
            GameObject playerByName = GameObject.Find("Player");
            if (playerByName != null)
            {
                playerController = playerByName.GetComponent<PlayerController>();
            }
        }
    }
    
    private void ActualizarBarraVida()
    {
        if (playerController == null || rellenoBarraVida == null) return;
        
        // Calcular el porcentaje de vida
        float porcentajeVida = Mathf.Clamp01(playerController.hp / maxVida);
        
        // Actualizar la barra de vida
        rellenoBarraVida.fillAmount = porcentajeVida;
        
        // Actualizar texto si existe
        if (vidaTexto != null)
        {
            vidaTexto.text = $"{playerController.hp:F0}/{maxVida:F0}";
        }
        

    }
    
   
    
    // Método público para forzar actualización (útil para eventos)
    public void ForzarActualizacion()
    {
        ActualizarBarraVida();
    }
    
    // Método para configurar vida máxima manualmente
    public void ConfigurarVidaMaxima(float nuevaVidaMaxima)
    {
        maxVida = nuevaVidaMaxima;
        ActualizarBarraVida();
    }
}
