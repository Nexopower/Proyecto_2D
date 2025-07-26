using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton para acceso global

    [Header("UI Elements")]
    public Text scoreText; // Texto para mostrar la puntuación
    public Text keysText; // Texto para mostrar el contador de llaves
    public Text healthText; // Texto para mostrar la vida

    [Header("Game Stats")]
    public int score = 0; // Puntuación actual
    public int keys = 0; // Número de llaves recolectadas
    
    [Header("Collectables Count")]
    public int coinsCollected = 0;
    public int gemsCollected = 0;
    public int redGemsCollected = 0;
    public int blueGemsCollected = 0;
    public int greenGemsCollected = 0;
    public int yellowGemsCollected = 0;
    
    [Header("Scene Management")]
    public string menuInicioScene = "MenuInicio";
    public string instructionsScene = "Instrucciones";
    public string winScene = "Win";
    public string loseScene = "Lose";
    public string levelPrefix = "Nivel"; // Prefijo para los niveles (Nivel1, Nivel2, etc.)
    public int currentLevel = 1; // Nivel actual
    public int maxLevels = 4; // Número máximo de niveles
    
    private PlayerController playerController; // Referencia al jugador

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Suscribirse al evento de carga de escenas
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            Debug.Log("GameManager creado y configurado como persistente");
        }
        else
        {
            Debug.Log("GameManager duplicado encontrado, destruyendo...");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Buscar el PlayerController en la escena
        FindPlayerController();
        
        // Actualizar la UI al inicio
        UpdateUI();
    }

    // Método para buscar el PlayerController (útil cuando cambia de escena)
    private void FindPlayerController()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController no encontrado en la escena actual");
        }
    }

    // Método público para refrescar la UI (llamado desde PersistentCanvas)
    public void RefreshUI()
    {
        FindPlayerController();
        UpdateUI();
    }

    // Método para añadir puntos
    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    // Método para añadir llaves
    public void AddKey()
    {
        keys++;
        UpdateUI();
    }
    
    // Método para añadir coleccionables
    public void AddCollectable(CollectableType type, int amount = 1)
    {
        switch (type)
        {
            case CollectableType.Coin:
                coinsCollected += amount;
                break;
            case CollectableType.Gem:
                gemsCollected += amount;
                break;
            case CollectableType.GemRed:
                redGemsCollected += amount;
                break;
            case CollectableType.GemBlue:
                blueGemsCollected += amount;
                break;
            case CollectableType.GemGreen:
                greenGemsCollected += amount;
                break;
            case CollectableType.GemYellow:
                yellowGemsCollected += amount;
                break;
        }
        
        Debug.Log($"Coleccionable añadido: {type} x{amount}");
        UpdateUI();
    }

    // Método para usar una llave
    public bool UseKey()
    {
        if (keys > 0)
        {
            keys--;
            UpdateUI();
            return true;
        }
        return false;
    }

    // Método para curar al jugador
    public void HealPlayer()
    {
        if (playerController != null)
        {
            playerController.hp++;
            UpdateUI();
        }
    }

    // Método para actualizar la UI
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
            
        if (keysText != null)
            keysText.text = "Keys: " + keys;
            
        if (healthText != null && playerController != null)
            healthText.text = "HP: " + playerController.hp;
    }

    private void Update()
    {
        // Actualizar la UI en cada frame si es necesario
        if (scoreText != null)
            scoreText.text = "Score: " + score;
        
        if (keysText != null)
            keysText.text = "Keys: " + keys;
        
        if (healthText != null && playerController != null)
            healthText.text = "HP: " + playerController.hp;
    }

    // Método para obtener el número de llaves (para el LockedBlock)
    public int GetKeyCount()
    {
        return keys;
    }
    
    // Métodos para obtener la cantidad de cada tipo de coleccionable
    public int GetCollectableCount(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin: return coinsCollected;
            case CollectableType.Gem: return gemsCollected;
            case CollectableType.GemRed: return redGemsCollected;
            case CollectableType.GemBlue: return blueGemsCollected;
            case CollectableType.GemGreen: return greenGemsCollected;
            case CollectableType.GemYellow: return yellowGemsCollected;
            case CollectableType.Key: return keys;
            default: return 0;
        }
    }
    
    // ============ MÉTODOS DE NAVEGACIÓN ENTRE ESCENAS ============
    
    // Evento que se ejecuta cuando se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Escena cargada: {scene.name}");
        
        // Asegurar que la instancia sigue siendo válida
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Reestableciendo instancia de GameManager");
        }
        
        // Refrescar UI después de un pequeño delay para asegurar que todo esté cargado
        Invoke(nameof(RefreshUI), 0.1f);
    }
    
    // ---- MÉTODOS PARA EL MENÚ DE INICIO ----
    
    /// <summary>
    /// Inicia el primer nivel del juego
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame llamado");
        if (Instance == null)
        {
            Debug.LogError("GameManager Instance es null!");
            return;
        }
        
        currentLevel = 1;
        LoadLevel(currentLevel);
        Debug.Log("Iniciando juego - Nivel 1");
    }
    
    /// <summary>
    /// Muestra la escena de instrucciones
    /// </summary>
    public void ShowInstructions()
    {
        Debug.Log("ShowInstructions llamado");
        if (Instance == null)
        {
            Debug.LogError("GameManager Instance es null!");
            return;
        }
        
        if (!string.IsNullOrEmpty(instructionsScene))
        {
            LoadScene(instructionsScene);
            Debug.Log("Mostrando instrucciones");
        }
        else
        {
            Debug.LogWarning("Escena de instrucciones no configurada");
        }
    }
    
    /// <summary>
    /// Sale del juego
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("ExitGame llamado");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // ---- MÉTODOS PARA LA PANTALLA DE VICTORIA (Win) ----
    
    /// <summary>
    /// Reinicia el nivel actual
    /// </summary>
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
        Debug.Log($"Reiniciando nivel {currentLevel}");
    }
    
    /// <summary>
    /// Avanza al siguiente nivel
    /// </summary>
    public void NextLevel()
    {
        Debug.Log($"NextLevel llamado. Nivel actual: {currentLevel}, Max niveles: {maxLevels}");
        
        if (currentLevel < maxLevels)
        {
            currentLevel++;
            LoadLevel(currentLevel);
            Debug.Log($"Avanzando al nivel {currentLevel}");
        }
        else
        {
            // Si ha completado todos los niveles, volver al menú
            Debug.Log("¡Juego completado! Volviendo al menú principal");
            BackToMenu();
        }
    }
    
    /// <summary>
    /// Vuelve al menú principal
    /// </summary>
    public void BackToMenu()
    {
        Debug.Log("BackToMenu llamado");
        if (Instance == null)
        {
            Debug.LogError("GameManager Instance es null!");
            return;
        }
        
        // Resetear estadísticas del juego
        ResetGameStats();
        LoadScene(menuInicioScene);
        Debug.Log("Volviendo al menú principal");
    }
    
    // ---- MÉTODOS PARA LA PANTALLA DE DERROTA (Lose) ----
    
    /// <summary>
    /// Vuelve al primer nivel después de perder
    /// </summary>
    public void BackToFirstLevel()
    {
        currentLevel = 1;
        LoadLevel(currentLevel);
        // Resetear estadísticas del juego
        ResetGameStats();
        Debug.Log("Volviendo al primer nivel");
    }
    
    // ---- MÉTODOS DE CONTROL DE JUEGO ----
    
    /// <summary>
    /// Método llamado cuando el jugador gana un nivel
    /// </summary>
    public void PlayerWon()
    {
        Debug.Log($"¡Nivel {currentLevel} completado!");
        LoadScene(winScene);
    }
    
    /// <summary>
    /// Método llamado cuando el jugador pierde
    /// </summary>
    public void PlayerLost()
    {
        Debug.Log("Jugador derrotado");
        LoadScene(loseScene);
    }
    
    /// <summary>
    /// Carga un nivel específico
    /// </summary>
    /// <param name="levelNumber">Número del nivel a cargar</param>
    public void LoadLevel(int levelNumber)
    {
        string levelName = levelPrefix + levelNumber;
        currentLevel = levelNumber;
        LoadScene(levelName);
        Debug.Log($"Cargando nivel: {levelName}");
    }
    
    /// <summary>
    /// Método genérico para cargar cualquier escena
    /// </summary>
    /// <param name="sceneName">Nombre de la escena a cargar</param>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Cargando escena: {sceneName}");
            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al cargar la escena {sceneName}: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Nombre de escena vacío o nulo");
        }
    }
    
    /// <summary>
    /// Verifica si el GameManager está funcionando correctamente
    /// </summary>
    public void VerifyGameManagerState()
    {
        Debug.Log($"=== ESTADO DEL GAMEMANAGER ===");
        Debug.Log($"Instance != null: {Instance != null}");
        Debug.Log($"GameObject activo: {gameObject.activeInHierarchy}");
        Debug.Log($"Nivel actual: {currentLevel}");
        Debug.Log($"Max niveles: {maxLevels}");
        Debug.Log($"Escena actual: {SceneManager.GetActiveScene().name}");
        Debug.Log($"==============================");
    }
    
    /// <summary>
    /// Resetea las estadísticas del juego
    /// </summary>
    private void ResetGameStats()
    {
        score = 0;
        keys = 0;
        coinsCollected = 0;
        gemsCollected = 0;
        redGemsCollected = 0;
        blueGemsCollected = 0;
        greenGemsCollected = 0;
        yellowGemsCollected = 0;
        UpdateUI();
        Debug.Log("Estadísticas del juego reseteadas");
    }
    
    /// <summary>
    /// Obtiene el número del nivel actual
    /// </summary>
    /// <returns>Número del nivel actual</returns>
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    /// <summary>
    /// Establece el número del nivel actual
    /// </summary>
    /// <param name="level">Número del nivel</param>
    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }
    
    /// <summary>
    /// Verifica si hay más niveles disponibles
    /// </summary>
    /// <returns>True si hay más niveles, false si no</returns>
    public bool HasNextLevel()
    {
        return currentLevel < maxLevels;
    }
    
    // ---- MÉTODOS ESTÁTICOS PARA ACCESO RÁPIDO ----
    
    /// <summary>
    /// Método estático para ir a la escena de victoria
    /// </summary>
    public static void GoToWinScene()
    {
        if (Instance != null)
        {
            Instance.PlayerWon();
        }
        else
        {
            SceneManager.LoadScene("Win");
        }
    }
    
    /// <summary>
    /// Método estático para ir a la escena de derrota
    /// </summary>
    public static void GoToLoseScene()
    {
        if (Instance != null)
        {
            Instance.PlayerLost();
        }
        else
        {
            SceneManager.LoadScene("Lose");
        }
    }
    
    /// <summary>
    /// Método estático para volver al menú principal
    /// </summary>
    public static void GoToMenuInicio()
    {
        if (Instance != null)
        {
            Instance.BackToMenu();
        }
        else
        {
            SceneManager.LoadScene("MenuInicio");
        }
    }
    
    // ---- CLEANUP ----
    
    private void OnDestroy()
    {
        // Desuscribirse del evento cuando se destruye el objeto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}