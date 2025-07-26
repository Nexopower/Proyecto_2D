using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [Header("Coin Settings")]
    public int scoreValue = 10; // Valor de puntos que otorga la moneda
    
    [Header("Audio")]
    public AudioClip collectSound; // Sonido al recolectar
    private AudioSource audioSource;

    [Header("Effects")]
    public ParticleSystem collectEffect; // Efecto de partículas al recolectar

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
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

        // Añadir puntos al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Destruir la moneda al recogerla
        Destroy(gameObject);
    }
}
