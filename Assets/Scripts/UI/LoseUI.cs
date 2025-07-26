using UnityEngine;
using UnityEngine.UI;

public class LoseUI : MonoBehaviour
{
    [Header("Botones de Derrota")]
    public Button botonVolverMenu;
    public Button botonVolverPrimerNivel;
    
    [Header("Elementos UI")]
    public Text textoDerrota;
    public Text textoMotivo; // Para mostrar por qué perdió
    
    void Start()
    {
        // Configurar los eventos de los botones
        ConfigurarBotones();
        
        // Mostrar información de derrota
        MostrarInformacionDerrota();
    }
    
    void ConfigurarBotones()
    {
        if (botonVolverMenu != null)
        {
            botonVolverMenu.onClick.RemoveAllListeners();
            botonVolverMenu.onClick.AddListener(OnVolverMenuClicked);
        }
        
        if (botonVolverPrimerNivel != null)
        {
            botonVolverPrimerNivel.onClick.RemoveAllListeners();
            botonVolverPrimerNivel.onClick.AddListener(OnVolverPrimerNivelClicked);
        }
        
        // Log de advertencia si falta GameManager
        if (GameManager.Instance == null)
            Debug.LogWarning("LoseUI: No se encontró GameManager.Instance");
    }
    
    void MostrarInformacionDerrota()
    {
        if (textoDerrota != null)
        {
            textoDerrota.text = "GAME OVER";
        }
        
        if (textoMotivo != null)
        {
            // Puedes personalizar el mensaje según cómo perdió el jugador
            textoMotivo.text = "¡Inténtalo de nuevo!";
        }
    }
    
    // Métodos que se pueden usar directamente desde los botones en el Inspector
    public void OnVolverMenuClicked()
    {
        Debug.Log("Botón Volver al Menú presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToMenu();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    public void OnVolverPrimerNivelClicked()
    {
        Debug.Log("Botón Volver al Primer Nivel presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToFirstLevel();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    // Método para personalizar el mensaje de derrota
    public void SetMotivoDerrota(string motivo)
    {
        if (textoMotivo != null)
        {
            textoMotivo.text = motivo;
        }
    }
}
