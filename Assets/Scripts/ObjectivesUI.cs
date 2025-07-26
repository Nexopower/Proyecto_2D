using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectivesUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject objectivePanel; // Panel que contiene todos los objetivos
    public GameObject objectivePrefab; // Prefab para mostrar cada objetivo individual
    public Transform objectivesContainer; // Container donde se instancian los objetivos
    
    [Header("Settings")]
    public bool showObjectivesOnStart = true;
    public bool updateInRealTime = true;
    public float updateInterval = 0.5f; // Intervalo de actualización en segundos
    
    private LevelObjectives levelObjectives;
    private List<ObjectiveUIItem> objectiveUIItems = new List<ObjectiveUIItem>();
    private float lastUpdateTime = 0f;
    
    private void Start()
    {
        // Buscar LevelObjectives en la escena
        levelObjectives = FindObjectOfType<LevelObjectives>();
        
        if (levelObjectives == null)
        {
            Debug.LogWarning("ObjectivesUI: No se encontró LevelObjectives en la escena");
            if (objectivePanel != null)
                objectivePanel.SetActive(false);
            return;
        }
        
        if (showObjectivesOnStart)
        {
            ShowObjectives();
        }
    }
    
    private void Update()
    {
        if (updateInRealTime && levelObjectives != null && Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateObjectivesDisplay();
            lastUpdateTime = Time.time;
        }
    }
    
    public void ShowObjectives()
    {
        if (levelObjectives == null || objectivesContainer == null)
            return;
            
        // Limpiar objetivos anteriores
        ClearObjectives();
        
        // Activar el panel de objetivos
        if (objectivePanel != null)
            objectivePanel.SetActive(true);
        
        // Crear UI para cada objetivo
        foreach (var objective in levelObjectives.objectives)
        {
            CreateObjectiveUI(objective);
        }
        
        // Actualizar el display inicial
        UpdateObjectivesDisplay();
    }
    
    private void CreateObjectiveUI(ObjectiveRequirement objective)
    {
        GameObject objectiveObj;
        
        if (objectivePrefab != null)
        {
            // Usar prefab personalizado
            objectiveObj = Instantiate(objectivePrefab, objectivesContainer);
        }
        else
        {
            // Crear UI básica
            objectiveObj = CreateBasicObjectiveUI();
        }
        
        // Obtener o agregar el componente ObjectiveUIItem
        ObjectiveUIItem uiItem = objectiveObj.GetComponent<ObjectiveUIItem>();
        if (uiItem == null)
        {
            uiItem = objectiveObj.AddComponent<ObjectiveUIItem>();
        }
        
        // Configurar el item
        uiItem.SetObjective(objective);
        objectiveUIItems.Add(uiItem);
    }
    
    private GameObject CreateBasicObjectiveUI()
    {
        // Crear GameObject básico para el objetivo
        GameObject obj = new GameObject("Objective");
        obj.transform.SetParent(objectivesContainer);
        
        // Agregar LayoutElement para mejor organización
        LayoutElement layoutElement = obj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 30f;
        
        // Agregar RectTransform
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return obj;
    }
    
    public void UpdateObjectivesDisplay()
    {
        foreach (var uiItem in objectiveUIItems)
        {
            if (uiItem != null)
            {
                uiItem.UpdateDisplay();
            }
        }
    }
    
    public void ClearObjectives()
    {
        foreach (var uiItem in objectiveUIItems)
        {
            if (uiItem != null && uiItem.gameObject != null)
            {
                Destroy(uiItem.gameObject);
            }
        }
        objectiveUIItems.Clear();
    }
    
    public void HideObjectives()
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(false);
    }
    
    public void ToggleObjectives()
    {
        if (objectivePanel != null)
        {
            bool isActive = objectivePanel.activeSelf;
            objectivePanel.SetActive(!isActive);
        }
    }
    
    // Método público para refrescar desde otros scripts
    public void RefreshObjectives()
    {
        ShowObjectives();
    }
}
