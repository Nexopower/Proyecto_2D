using UnityEngine;

/// <summary>
/// Script temporal para debuggear el GameManager
/// Agrégalo a cualquier GameObject para verificar el estado
/// </summary>
public class GameManagerDebugger : MonoBehaviour
{
    [Header("Debug Controls")]
    public bool autoDebugOnStart = true;
    public KeyCode debugKey = KeyCode.F12;
    
    private void Start()
    {
        if (autoDebugOnStart)
        {
            Invoke(nameof(DebugGameManager), 1f);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugGameManager();
        }
    }
    
    public void DebugGameManager()
    {
        Debug.Log("=== GAMEMANAGER DEBUGGER ===");
        
        if (GameManager.Instance != null)
        {
            Debug.Log("✓ GameManager Instance existe");
            GameManager.Instance.VerifyGameManagerState();
        }
        else
        {
            Debug.LogError("✗ GameManager Instance es NULL!");
            
            // Buscar si hay algún GameManager en la escena
            GameManager[] managers = FindObjectsOfType<GameManager>();
            Debug.Log($"GameManagers encontrados en la escena: {managers.Length}");
            
            for (int i = 0; i < managers.Length; i++)
            {
                Debug.Log($"GameManager {i}: {managers[i].name} - Activo: {managers[i].gameObject.activeInHierarchy}");
            }
        }
        
        Debug.Log("============================");
    }
    
    // Métodos de prueba para los botones
    public void TestStartGame()
    {
        Debug.Log("Probando StartGame...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("No se puede probar StartGame - GameManager Instance es null");
        }
    }
    
    public void TestShowInstructions()
    {
        Debug.Log("Probando ShowInstructions...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowInstructions();
        }
        else
        {
            Debug.LogError("No se puede probar ShowInstructions - GameManager Instance es null");
        }
    }
    
    public void TestBackToMenu()
    {
        Debug.Log("Probando BackToMenu...");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToMenu();
        }
        else
        {
            Debug.LogError("No se puede probar BackToMenu - GameManager Instance es null");
        }
    }
}
