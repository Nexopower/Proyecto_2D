using UnityEngine;
using UnityEngine.UI;

public class MenuInicioUI : MonoBehaviour
{
    [Header("Botones del Menú")]
    public Button botonJugar;
    public Button botonInstrucciones;
    public Button botonSalir;
    
    void Start()
    {
        // Configurar los eventos de los botones
        ConfigurarBotones();
    }
    
    void ConfigurarBotones()
    {
        if (botonJugar != null)
        {
            botonJugar.onClick.RemoveAllListeners();
            botonJugar.onClick.AddListener(OnJugarClicked);
        }
        
        if (botonInstrucciones != null)
        {
            botonInstrucciones.onClick.RemoveAllListeners();
            botonInstrucciones.onClick.AddListener(OnInstruccionesClicked);
        }
        
        if (botonSalir != null)
        {
            botonSalir.onClick.RemoveAllListeners();
            botonSalir.onClick.AddListener(OnSalirClicked);
        }
    }
    
    // Métodos que se pueden usar directamente desde los botones en el Inspector
    public void OnJugarClicked()
    {
        Debug.Log("Botón Jugar presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    public void OnInstruccionesClicked()
    {
        Debug.Log("Botón Instrucciones presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowInstructions();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
        }
    }
    
    public void OnSalirClicked()
    {
        Debug.Log("Botón Salir presionado");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExitGame();
        }
        else
        {
            Debug.LogError("GameManager.Instance es null!");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
