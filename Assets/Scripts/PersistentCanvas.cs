using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCanvas : MonoBehaviour
{
    public static PersistentCanvas Instance;
    
    [Header("UI References")]
    public GameObject uiPanel; // Panel que contiene toda la UI del juego
    public ObjectivesUI objectivesUI; // Referencia al sistema de objetivos UI
    
    private void Awake()
    {
        // Singleton para el Canvas persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configurar el sort order inicial
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 1;
            }
            
            // Suscribirse a eventos de cambio de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Asegurar que el Canvas esté en el orden correcto
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 1; // Establecer el sort order en 1
        }
        
        // Reactivar la UI si fue desactivada
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
        
        // Actualizar referencias del GameManager si es necesario
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RefreshUI();
        }
        
        // Refrescar objetivos UI si están disponibles
        if (objectivesUI != null)
        {
            // Esperar un frame para que todo esté inicializado
            StartCoroutine(RefreshObjectivesDelayed());
        }
    }
    
    private System.Collections.IEnumerator RefreshObjectivesDelayed()
    {
        yield return new WaitForEndOfFrame();
        if (objectivesUI != null)
        {
            objectivesUI.RefreshObjectives();
        }
    }
    
    // Método para ocultar la UI temporalmente (útil para cutscenes, menús, etc.)
    public void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }
    
    // Método para mostrar la UI
    public void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }
    
    // Método para toggle de objetivos
    public void ToggleObjectives()
    {
        if (objectivesUI != null)
        {
            objectivesUI.ToggleObjectives();
        }
    }
    
    // Método para mostrar objetivos
    public void ShowObjectives()
    {
        if (objectivesUI != null)
        {
            objectivesUI.ShowObjectives();
        }
    }
    
    // Método para ocultar objetivos
    public void HideObjectives()
    {
        if (objectivesUI != null)
        {
            objectivesUI.HideObjectives();
        }
    }
    
    private void OnDestroy()
    {
        // Desuscribirse de eventos para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
