using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Collectable Settings")]
    public CollectableType itemType; // Tipo de objeto coleccionable
    public int scoreValue = 10; // Valor de puntos que otorga
    
    [Header("Audio")]
    public AudioClip collectSound; // Sonido al recolectar
    private AudioSource audioSource;

    [Header("Effects")]
    public ParticleSystem collectEffect; // Efecto de partículas al recolectar
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        // Reproducir sonido si está disponible
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Crear efecto de partículas si está disponible
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, transform.rotation);
        }

        // Manejar diferentes tipos de objetos
        switch (itemType)
        {
            case CollectableType.Coin:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.Gem:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.GemRed:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.GemBlue:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.GemGreen:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.GemYellow:
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.AddCollectable(itemType, 1);
                break;
                
            case CollectableType.Key:
                GameManager.Instance.AddKey();
                break;
                
            case CollectableType.Heart:
                GameManager.Instance.HealPlayer();
                break;
        }

        // Destruir el objeto
        Destroy(gameObject);
    }
}

// Enumeración para los tipos de objetos coleccionables
public enum CollectableType
{
    Coin,       // Moneda
    Gem,        // Gema genérica
    GemRed,     // Gema roja
    GemBlue,    // Gema azul
    GemGreen,   // Gema verde
    GemYellow,  // Gema amarilla
    Key,        // Llave
    Heart       // Corazón
}