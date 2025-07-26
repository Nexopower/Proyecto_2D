using UnityEngine;
using UnityEngine.UI;

public class SimpleObjectivesDisplay : MonoBehaviour
{
    [Header("Simple Objectives Display")]
    public Text objectivesText; // Un solo texto que muestra todos los objetivos
    public bool showOnStart = true;
    public bool updateContinuously = true;
    
    [Header("Visual Settings")]
    public string completedSymbol = "✓";
    public string incompleteSymbol = "○";
    public Color completedColor = Color.green;
    public Color incompleteColor = Color.white;
    
    private LevelObjectives levelObjectives;
    
    private void Start()
    {
        // Buscar LevelObjectives en la escena
        levelObjectives = FindObjectOfType<LevelObjectives>();
        
        if (levelObjectives == null)
        {
            Debug.LogWarning("SimpleObjectivesDisplay: No se encontró LevelObjectives en la escena");
            if (objectivesText != null)
                objectivesText.text = "No hay objetivos configurados";
            return;
        }
        
        if (showOnStart)
        {
            UpdateObjectivesDisplay();
        }
    }
    
    private void Update()
    {
        if (updateContinuously && levelObjectives != null)
        {
            UpdateObjectivesDisplay();
        }
    }
    
    public void UpdateObjectivesDisplay()
    {
        if (levelObjectives == null || objectivesText == null) return;
        
        string objectivesString = "OBJETIVOS:\n";
        bool allCompleted = true;
        
        for (int i = 0; i < levelObjectives.objectives.Length; i++)
        {
            var objective = levelObjectives.objectives[i];
            string symbol = objective.IsCompleted ? completedSymbol : incompleteSymbol;
            string description = GetObjectiveDescription(objective);
            string progress = $"({objective.CurrentAmount}/{objective.requiredAmount})";
            
            objectivesString += $"{symbol} {description} {progress}\n";
            
            if (!objective.IsCompleted)
                allCompleted = false;
        }
        
        objectivesText.text = objectivesString;
        
        // Cambiar color del texto según si todos están completados
        objectivesText.color = allCompleted ? completedColor : incompleteColor;
    }
    
    private string GetObjectiveDescription(ObjectiveRequirement objective)
    {
        switch (objective.objectiveType)
        {
            case ObjectiveType.Score:
                return $"Puntos: {objective.requiredAmount}";
                
            case ObjectiveType.Keys:
                return $"Llaves: {objective.requiredAmount}";
                
            case ObjectiveType.Collectable:
                string collectableName = GetCollectableName(objective.collectableType);
                return $"{collectableName}: {objective.requiredAmount}";
                
            default:
                return "Objetivo desconocido";
        }
    }
    
    private string GetCollectableName(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin: return "Monedas";
            case CollectableType.Gem: return "Gemas";
            case CollectableType.GemRed: return "Gemas Rojas";
            case CollectableType.GemBlue: return "Gemas Azules";
            case CollectableType.GemGreen: return "Gemas Verdes";
            case CollectableType.GemYellow: return "Gemas Amarillas";
            case CollectableType.Key: return "Llaves";
            case CollectableType.Heart: return "Corazones";
            default: return "Objetos";
        }
    }
    
    // Método público para refrescar desde otros scripts
    public void RefreshDisplay()
    {
        UpdateObjectivesDisplay();
    }
}
