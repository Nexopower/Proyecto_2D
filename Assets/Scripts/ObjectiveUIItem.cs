using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIItem : MonoBehaviour
{
    [Header("UI Components")]
    public Text objectiveText; // Texto que describe el objetivo
    public Text progressText; // Texto que muestra el progreso (ej: "2/5")
    public Image progressBar; // Barra de progreso visual (opcional)
    public Image statusIcon; // Icono de estado (completado/pendiente)
    
    [Header("Visual Settings")]
    public Color completedColor = Color.green;
    public Color incompleteColor = Color.white;
    public Color progressBarColor = Color.yellow;
    
    private ObjectiveRequirement objective;
    private bool isSetup = false;
    
    private void Awake()
    {
        SetupUIComponents();
    }
    
    private void SetupUIComponents()
    {
        if (isSetup) return;
        
        // Si no hay componentes asignados, intentar encontrarlos automáticamente
        if (objectiveText == null)
        {
            objectiveText = GetComponentInChildren<Text>();
            
            // Si no encuentra ningún Text, crear uno
            if (objectiveText == null)
            {
                CreateBasicTextComponents();
            }
        }
        
        isSetup = true;
    }
    
    private void CreateBasicTextComponents()
    {
        // Crear texto principal del objetivo
        GameObject textObj = new GameObject("ObjectiveText");
        textObj.transform.SetParent(transform);
        
        objectiveText = textObj.AddComponent<Text>();
        objectiveText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        objectiveText.fontSize = 14;
        objectiveText.color = Color.white;
        objectiveText.alignment = TextAnchor.MiddleLeft;
        
        // Configurar RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.localScale = Vector3.one;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);
        
        // Crear texto de progreso
        GameObject progressObj = new GameObject("ProgressText");
        progressObj.transform.SetParent(transform);
        
        progressText = progressObj.AddComponent<Text>();
        progressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        progressText.fontSize = 12;
        progressText.color = Color.yellow;
        progressText.alignment = TextAnchor.MiddleRight;
        
        // Configurar RectTransform para el texto de progreso
        RectTransform progressRect = progressObj.GetComponent<RectTransform>();
        progressRect.localScale = Vector3.one;
        progressRect.anchorMin = new Vector2(0.7f, 0);
        progressRect.anchorMax = Vector2.one;
        progressRect.offsetMin = Vector2.zero;
        progressRect.offsetMax = new Vector2(-10, 0);
    }
    
    public void SetObjective(ObjectiveRequirement obj)
    {
        objective = obj;
        SetupUIComponents();
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        if (objective == null) return;
        
        // Actualizar texto del objetivo
        if (objectiveText != null)
        {
            string description = GetObjectiveDescription();
            string status = objective.IsCompleted ? "✓" : "○";
            objectiveText.text = $"{status} {description}";
            
            // Cambiar color según el estado
            objectiveText.color = objective.IsCompleted ? completedColor : incompleteColor;
        }
        
        // Actualizar texto de progreso
        if (progressText != null)
        {
            progressText.text = $"{objective.CurrentAmount}/{objective.requiredAmount}";
            progressText.color = objective.IsCompleted ? completedColor : progressBarColor;
        }
        
        // Actualizar barra de progreso
        if (progressBar != null)
        {
            float progress = objective.requiredAmount > 0 ? 
                (float)objective.CurrentAmount / objective.requiredAmount : 0f;
            progressBar.fillAmount = Mathf.Clamp01(progress);
            progressBar.color = objective.IsCompleted ? completedColor : progressBarColor;
        }
        
        // Actualizar icono de estado
        if (statusIcon != null)
        {
            statusIcon.color = objective.IsCompleted ? completedColor : incompleteColor;
        }
    }
    
    private string GetObjectiveDescription()
    {
        if (objective == null) return "Objetivo desconocido";
        
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
}
