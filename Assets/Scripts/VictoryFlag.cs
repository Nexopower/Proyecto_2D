using UnityEngine;

public class VictoryFlag : MonoBehaviour
{
    [Header("Victory Flag Settings")]
    public bool requireObjectives = true; // Si debe verificar objetivos
    
    [Header("Audio & Effects")]
    public AudioClip victorySound;
    public ParticleSystem victoryEffect;
    
    [Header("Visual Feedback")]
    public GameObject completedVisual; // GameObject a activar cuando se cumplan objetivos
    public GameObject incompleteVisual; // GameObject a activar cuando falten objetivos
    
    private AudioSource audioSource;
    private LevelObjectives levelObjectives;
    private bool hasTriggered = false;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Buscar el componente LevelObjectives en la escena
        levelObjectives = FindObjectOfType<LevelObjectives>();
        
        if (requireObjectives && levelObjectives == null)
        {
            Debug.LogWarning("VictoryFlag: No se encontró LevelObjectives en la escena. Se desactivará la verificación de objetivos.");
            requireObjectives = false;
        }
        
        // Configurar visuales iniciales
        UpdateVisuals();
    }
    
    private void Update()
    {
        // Actualizar visuales basados en el progreso de objetivos
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (!requireObjectives || levelObjectives == null) return;
        
        bool objectivesCompleted = levelObjectives.AreAllObjectivesCompleted();
        
        if (completedVisual != null)
        {
            completedVisual.SetActive(objectivesCompleted);
        }
        
        if (incompleteVisual != null)
        {
            incompleteVisual.SetActive(!objectivesCompleted);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return; // Evitar múltiples activaciones
        
        if (collision.CompareTag("Player"))
        {
            TryCompleteLevel();
        }
    }
    
    private void TryCompleteLevel()
    {
        // Verificar si se deben cumplir objetivos
        if (requireObjectives && levelObjectives != null)
        {
            if (levelObjectives.AreAllObjectivesCompleted())
            {
                CompleteLevel();
            }
            else
            {
                ShowIncompleteObjectives();
            }
        }
        else
        {
            // No hay objetivos requeridos, completar nivel directamente
            CompleteLevel();
        }
    }
    
    private void CompleteLevel()
    {
        hasTriggered = true;
        
        Debug.Log("¡Nivel completado! ¡Victoria!");
        
        // Reproducir sonido de victoria
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
        
        // Crear efecto de victoria
        if (victoryEffect != null)
        {
            ParticleSystem effect = Instantiate(victoryEffect, transform.position, transform.rotation);
            Destroy(effect.gameObject, 3f); // Destruir el efecto después de 3 segundos
        }
        
        // Mostrar objetivos completados
        if (levelObjectives != null)
        {
            levelObjectives.ShowObjectivesProgress();
        }
        
        // Esperar un poco antes de cambiar de escena para que el jugador vea el efecto
        Invoke(nameof(LoadVictoryScene), 1.5f);
    }
    
    private void ShowIncompleteObjectives()
    {
        Debug.Log("❌ No se puede completar el nivel. Objetivos pendientes:");
        
        if (levelObjectives != null)
        {
            levelObjectives.ShowObjectivesProgress();
        }
        
        // Opcional: Mostrar un efecto visual o sonido de "no completado"
        // Aquí puedes agregar feedback visual/audio para indicar que faltan objetivos
    }
    
    private void LoadVictoryScene()
    {
        // Usar el GameManager para ir a la escena de victoria
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerWon();
        }
        else
        {
            Debug.LogError("GameManager.Instance no encontrado!");
        }
    }
    
    /// <summary>
    /// Método para forzar la victoria (útil para testing)
    /// </summary>
    [ContextMenu("Force Victory")]
    public void ForceVictory()
    {
        Debug.Log("Forzando victoria...");
        CompleteLevel();
    }
    
    /// <summary>
    /// Método para mostrar objetivos actuales (útil para testing)
    /// </summary>
    [ContextMenu("Show Objectives")]
    public void ShowCurrentObjectives()
    {
        if (levelObjectives != null)
        {
            levelObjectives.ShowObjectivesProgress();
        }
        else
        {
            Debug.Log("No hay LevelObjectives configurado en la escena");
        }
    }
}
