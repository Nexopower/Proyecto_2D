using UnityEngine;

public class LockedBlock : MonoBehaviour
{
    [Header("Block Settings")]
    public bool requiresKey = true; // Si requiere llave para ser desbloqueado
    public int keysRequired = 1; // Número de llaves necesarias
    
    [Header("Visual Feedback")]
    public GameObject lockedSprite; // Sprite cuando está bloqueado
    public GameObject unlockedSprite; // Sprite cuando está desbloqueado
    
    [Header("Audio")]
    public AudioClip unlockSound; // Sonido al desbloquear
    public AudioClip deniedSound; // Sonido cuando no tiene llaves
    private AudioSource audioSource;

    [Header("Effects")]
    public ParticleSystem unlockEffect; // Efecto al desbloquear
    
    private bool isUnlocked = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateVisuals();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isUnlocked)
        {
            AttemptUnlock();
        }
    }

    private void AttemptUnlock()
    {
        // Verificar si el GameManager existe
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager no encontrado!");
            return;
        }

        // Verificar si el jugador tiene suficientes llaves
        if (GameManager.Instance.GetKeyCount() >= keysRequired)
        {
            // Usar las llaves necesarias
            for (int i = 0; i < keysRequired; i++)
            {
                GameManager.Instance.UseKey();
            }

            // Desbloquear el bloque
            UnlockBlock();
        }
        else
        {
            // No tiene suficientes llaves
            PlayDeniedFeedback();
        }
    }

    private void UnlockBlock()
    {
        isUnlocked = true;

        // Reproducir sonido de desbloqueo
        if (unlockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        // Crear efecto de desbloqueo
        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, transform.position, transform.rotation);
        }

        // Actualizar visuales
        UpdateVisuals();

        // Destruir el bloque después de un pequeño delay para permitir efectos
        Invoke("DestroyBlock", 0.5f);
    }

    private void PlayDeniedFeedback()
    {
        // Reproducir sonido de denegación
        if (deniedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deniedSound);
        }

        // Aquí podrías añadir una animación de "sacudida" o cambio de color temporal
        Debug.Log("¡Necesitas " + keysRequired + " llave(s) para abrir este bloque!");
    }

    private void UpdateVisuals()
    {
        if (lockedSprite != null)
            lockedSprite.SetActive(!isUnlocked);
            
        if (unlockedSprite != null)
            unlockedSprite.SetActive(isUnlocked);
    }

    private void DestroyBlock()
    {
        Destroy(gameObject);
    }

    // Método para forzar el desbloqueo (útil para testing o eventos especiales)
    public void ForceUnlock()
    {
        UnlockBlock();
    }

    // Método para verificar si está desbloqueado
    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}