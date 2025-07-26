using UnityEngine;
using System;

[Serializable]
public class ObjectiveRequirement
{
    [Header("Objective Type")]
    public ObjectiveType objectiveType;
    
    [Header("Requirements")]
    public int requiredAmount = 0;
    public CollectableType collectableType; // Solo usado si el tipo es Collectable
    
    [Header("Current Progress")]
    [SerializeField] private int currentAmount = 0;
    
    public int CurrentAmount => currentAmount;
    public bool IsCompleted => currentAmount >= requiredAmount;
    
    public void UpdateProgress(int newAmount)
    {
        currentAmount = newAmount;
    }
    
    public void ResetProgress()
    {
        currentAmount = 0;
    }
}

public enum ObjectiveType
{
    Score,          // Puntuación mínima
    Collectable,    // Cantidad específica de un coleccionable
    Keys            // Número de llaves
}

public class LevelObjectives : MonoBehaviour
{
    [Header("Level Objectives Configuration")]
    public ObjectiveRequirement[] objectives;
    
    [Header("Objective Display")]
    public bool showObjectivesInConsole = true;
    
    private void Start()
    {
        if (showObjectivesInConsole)
        {
            ShowObjectivesList();
        }
    }
    
    private void Update()
    {
        // Actualizar el progreso de los objetivos cada frame
        UpdateObjectivesProgress();
    }
    
    private void UpdateObjectivesProgress()
    {
        if (GameManager.Instance == null) return;
        
        foreach (var objective in objectives)
        {
            switch (objective.objectiveType)
            {
                case ObjectiveType.Score:
                    objective.UpdateProgress(GameManager.Instance.score);
                    break;
                    
                case ObjectiveType.Keys:
                    objective.UpdateProgress(GameManager.Instance.keys);
                    break;
                    
                case ObjectiveType.Collectable:
                    int count = GetCollectableCount(objective.collectableType);
                    objective.UpdateProgress(count);
                    break;
            }
        }
    }
    
    private int GetCollectableCount(CollectableType type)
    {
        if (GameManager.Instance == null) return 0;
        
        switch (type)
        {
            case CollectableType.Coin:
                return GameManager.Instance.coinsCollected;
            case CollectableType.Gem:
                return GameManager.Instance.gemsCollected;
            case CollectableType.GemRed:
                return GameManager.Instance.redGemsCollected;
            case CollectableType.GemBlue:
                return GameManager.Instance.blueGemsCollected;
            case CollectableType.GemGreen:
                return GameManager.Instance.greenGemsCollected;
            case CollectableType.GemYellow:
                return GameManager.Instance.yellowGemsCollected;
            case CollectableType.Key:
                return GameManager.Instance.keys;
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// Verifica si todos los objetivos han sido completados
    /// </summary>
    /// <returns>True si todos los objetivos están completos</returns>
    public bool AreAllObjectivesCompleted()
    {
        if (objectives == null || objectives.Length == 0)
        {
            Debug.LogWarning("No hay objetivos configurados para este nivel");
            return true; // Si no hay objetivos, se considera completado
        }
        
        foreach (var objective in objectives)
        {
            if (!objective.IsCompleted)
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Muestra la lista de objetivos en la consola
    /// </summary>
    public void ShowObjectivesList()
    {
        Debug.Log("=== OBJETIVOS DEL NIVEL ===");
        
        if (objectives == null || objectives.Length == 0)
        {
            Debug.Log("No hay objetivos configurados");
            return;
        }
        
        for (int i = 0; i < objectives.Length; i++)
        {
            var obj = objectives[i];
            string description = GetObjectiveDescription(obj);
            string status = obj.IsCompleted ? "✓ COMPLETADO" : "○ PENDIENTE";
            
            Debug.Log($"{i + 1}. {description} [{obj.CurrentAmount}/{obj.requiredAmount}] {status}");
        }
        
        Debug.Log("===========================");
    }
    
    /// <summary>
    /// Obtiene una descripción legible del objetivo
    /// </summary>
    private string GetObjectiveDescription(ObjectiveRequirement objective)
    {
        switch (objective.objectiveType)
        {
            case ObjectiveType.Score:
                return $"Obtener {objective.requiredAmount} puntos";
                
            case ObjectiveType.Keys:
                return $"Recolectar {objective.requiredAmount} llaves";
                
            case ObjectiveType.Collectable:
                string collectableName = GetCollectableName(objective.collectableType);
                return $"Recolectar {objective.requiredAmount} {collectableName}";
                
            default:
                return "Objetivo desconocido";
        }
    }
    
    /// <summary>
    /// Obtiene el nombre legible del coleccionable
    /// </summary>
    private string GetCollectableName(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin: return "monedas";
            case CollectableType.Gem: return "gemas";
            case CollectableType.GemRed: return "gemas rojas";
            case CollectableType.GemBlue: return "gemas azules";
            case CollectableType.GemGreen: return "gemas verdes";
            case CollectableType.GemYellow: return "gemas amarillas";
            case CollectableType.Key: return "llaves";
            case CollectableType.Heart: return "corazones";
            default: return "objetos";
        }
    }
    
    /// <summary>
    /// Muestra el progreso actual de los objetivos
    /// </summary>
    public void ShowObjectivesProgress()
    {
        ShowObjectivesList();
    }
    
    /// <summary>
    /// Resetea el progreso de todos los objetivos
    /// </summary>
    public void ResetObjectives()
    {
        foreach (var objective in objectives)
        {
            objective.ResetProgress();
        }
        
        Debug.Log("Objetivos reseteados");
    }
}
