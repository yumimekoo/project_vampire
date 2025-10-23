using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemiesPerWave = 1;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnRadius = 3f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private float nextSpawnTime;

    void Start()
    {
        if (playerTransform == null)
        {
            // Versucht automatisch, den Player zu finden
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            else
                Debug.LogWarning("EnemySpawner: Kein Player gefunden! Bitte manuell zuweisen.");
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnWave();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 worldPos = new Vector3(spawnPos.x, spawnPos.y, 0) + transform.position;

            GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
                enemyBase.Initialize(playerTransform);
        }
    }
}
