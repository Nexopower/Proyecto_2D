using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject coinPrefab;
    public GameObject gemPrefab;
    public GameObject keyPrefab;
    public GameObject heartPrefab;
    
    [Header("Spawn Settings")]
    public float spawnRadius = 5f;
    public int maxItems = 10;
    
    [Header("Spawn Quantities")]
    public int coinsToSpawn = 5;
    public int gemsToSpawn = 2;
    public int keysToSpawn = 1;
    public int heartsToSpawn = 1;

    private void Start()
    {
        SpawnCollectables();
    }

    public void SpawnCollectables()
    {
        // Spawnar monedas
        for (int i = 0; i < coinsToSpawn; i++)
        {
            SpawnItem(coinPrefab);
        }

        // Spawnar gemas
        for (int i = 0; i < gemsToSpawn; i++)
        {
            SpawnItem(gemPrefab);
        }

        // Spawnar llaves
        for (int i = 0; i < keysToSpawn; i++)
        {
            SpawnItem(keyPrefab);
        }

        // Spawnar corazones
        for (int i = 0; i < heartsToSpawn; i++)
        {
            SpawnItem(heartPrefab);
        }
    }

    private void SpawnItem(GameObject prefab)
    {
        if (prefab != null)
        {
            Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el radio de spawn en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
