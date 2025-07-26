using UnityEngine;

public static class CollectableHelper
{
    // Constantes para valores por defecto
    public static class DefaultValues
    {
        public const int COIN_SCORE = 10;
        public const int GEM_SCORE = 20;
        public const int HEART_HEAL = 1;
    }

    // Método para configurar automáticamente un objeto coleccionable
    public static void SetupCollectable(GameObject obj, CollectableType type, int scoreValue = 0)
    {
        // Añadir CollectableItem si no existe
        CollectableItem collectable = obj.GetComponent<CollectableItem>();
        if (collectable == null)
        {
            collectable = obj.AddComponent<CollectableItem>();
        }

        // Configurar el tipo
        collectable.itemType = type;

        // Configurar valor según el tipo
        switch (type)
        {
            case CollectableType.Coin:
                collectable.scoreValue = scoreValue > 0 ? scoreValue : DefaultValues.COIN_SCORE;
                break;
            case CollectableType.Gem:
                collectable.scoreValue = scoreValue > 0 ? scoreValue : DefaultValues.GEM_SCORE;
                break;
            case CollectableType.Key:
            case CollectableType.Heart:
                collectable.scoreValue = 0; // Llaves y corazones no dan puntos
                break;
        }

        // Asegurar que tiene un Collider2D como Trigger
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = obj.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;

        // Añadir AudioSource si no existe
        if (obj.GetComponent<AudioSource>() == null)
        {
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    // Método para crear un objeto coleccionable desde código
    public static GameObject CreateCollectable(CollectableType type, Vector3 position, Sprite sprite = null)
    {
        GameObject obj = new GameObject($"{type}_Collectable");
        obj.transform.position = position;

        // Añadir SpriteRenderer si se proporciona sprite
        if (sprite != null)
        {
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }

        // Configurar como coleccionable
        SetupCollectable(obj, type);

        return obj;
    }

    // Método para validar la configuración del GameManager
    public static bool ValidateGameManager()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager no encontrado en la escena. Asegúrate de tener un GameManager activo.");
            return false;
        }
        return true;
    }

    // Método para obtener información de un tipo de coleccionable
    public static string GetCollectableInfo(CollectableType type)
    {
        switch (type)
        {
            case CollectableType.Coin:
                return $"Moneda - Otorga {DefaultValues.COIN_SCORE} puntos";
            case CollectableType.Gem:
                return $"Gema - Otorga {DefaultValues.GEM_SCORE} puntos";
            case CollectableType.Key:
                return "Llave - Se almacena en el inventario para abrir bloques";
            case CollectableType.Heart:
                return $"Corazón - Restaura {DefaultValues.HEART_HEAL} punto de vida";
            default:
                return "Tipo desconocido";
        }
    }
}