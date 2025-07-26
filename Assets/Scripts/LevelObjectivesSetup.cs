using UnityEngine;

public class LevelObjectivesSetup : MonoBehaviour
{
    [ContextMenu("Create Basic Objectives")]
    public void CreateBasicObjectives()
    {
        // Buscar si ya existe un LevelObjectives
        LevelObjectives existing = FindObjectOfType<LevelObjectives>();
        if (existing != null)
        {
            Debug.Log("Ya existe un LevelObjectives en la escena");
            return;
        }
        
        // Crear GameObject para objetivos
        GameObject objectivesObj = new GameObject("LevelObjectives");
        LevelObjectives levelObjectives = objectivesObj.AddComponent<LevelObjectives>();
        
        // Crear objetivos básicos
        levelObjectives.objectives = new ObjectiveRequirement[3];
        
        // Objetivo 1: Puntuación
        levelObjectives.objectives[0] = new ObjectiveRequirement();
        levelObjectives.objectives[0].objectiveType = ObjectiveType.Score;
        levelObjectives.objectives[0].requiredAmount = 100;
        
        // Objetivo 2: Monedas
        levelObjectives.objectives[1] = new ObjectiveRequirement();
        levelObjectives.objectives[1].objectiveType = ObjectiveType.Collectable;
        levelObjectives.objectives[1].collectableType = CollectableType.Coin;
        levelObjectives.objectives[1].requiredAmount = 5;
        
        // Objetivo 3: Gemas rojas
        levelObjectives.objectives[2] = new ObjectiveRequirement();
        levelObjectives.objectives[2].objectiveType = ObjectiveType.Collectable;
        levelObjectives.objectives[2].collectableType = CollectableType.GemRed;
        levelObjectives.objectives[2].requiredAmount = 3;
        
        Debug.Log("Objetivos básicos creados! Revisa el GameObject 'LevelObjectives' en la Hierarchy");
    }
}
