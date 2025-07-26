using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [Header("Botones de Victoria")]
    public Button botonRepetirNivel;
    public Button botonSiguienteNivel;
    public Button botonVolverInicio;
    
    [Header("Elementos UI")]
    public Text textoVictoria;
    public Text textoPuntuacion; // Para mostrar puntuación final
    
    void Start()
    {
        // Configurar los eventos de los botones
        ConfigurarBotones();
        
        // Mostrar información de victoria
        MostrarInformacionVictoria();
    }
    
    void ConfigurarBotones()
    {
        if (botonRepetirNivel != null)
        {
            botonRepetirNivel.onClick.RemoveAllListeners();
            botonRepetirNivel.onClick.AddListener(OnRepetirNivelClicked);
        }
        
        if (botonSiguienteNivel != null)
        {
            botonSiguienteNivel.onClick.RemoveAllListeners();
            botonSiguienteNivel.onClick.AddListener(OnSiguienteNivelClicked);
        }
        
        if (botonVolverInicio != null)
        {
            botonVolverInicio.onClick.RemoveAllListeners();
            botonVolverInicio.onClick.AddListener(OnVolverInicioClicked);
        }
        
        // Log de advertencia si falta GameManager
        if (GameManager.Instance == null)
            Debug.LogWarning("WinUI: No se encontró GameManager.Instance");
    }
    
    void MostrarInformacionVictoria()
    {
        if (textoVictoria != null)
        {
            textoVictoria.text = "¡VICTORIA!";
        }
        
        if (textoPuntuacion != null)
        {
            // Obtener puntuación del GameManager si existe
            if (GameManager.Instance != null)
            {
                textoPuntuacion.text = $"Puntuación: {GameManager.Instance.score}";
            }
            else
            {
                textoPuntuacion.text = "Puntuación: --";
            }
        }
    }
    
    // Métodos que se pueden usar directamente desde los botones en el Inspector
    public void OnRepetirNivelClicked()
    {
        Debug.Log("Botón Repetir Nivel presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    public void OnSiguienteNivelClicked()
    {
        Debug.Log("Botón Siguiente Nivel presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    public void OnVolverInicioClicked()
    {
        Debug.Log("Botón Volver al Inicio presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToMenu();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
}
