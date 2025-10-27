using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[Header("Spawn Settings")]
    //[SerializeField] private GameObject enemyPrefab;
    //[SerializeField] private GameObject enemyPrefab2;
    //[SerializeField] private GameObject enemyPrefab3;
    //[SerializeField] private GameObject enemyPrefab4;
    //[SerializeField] private int enemiesPerWave = 1;
    //[SerializeField] private float spawnInterval = 1f;
    //[SerializeField] private float spawnRadius = 3f;

    //[Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyBulletPool pool;

    //private float nextSpawnTime;

    //void Start()
    //{
    //    if (playerTransform == null)
    //    {
    //        // Versucht automatisch, den Player zu finden
    //        GameObject player = GameObject.FindGameObjectWithTag("Player");
    //        if (player != null)
    //            playerTransform = player.transform;
    //        else
    //            Debug.LogWarning("EnemySpawner: Kein Player gefunden! Bitte manuell zuweisen.");
    //    }

    //    nextSpawnTime = Time.time + spawnInterval;
    //}

    //internal void StartSpawning(WaveInfo currentWave)
    //{
    //    throw new System.NotImplementedException();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Time.time >= nextSpawnTime)
    //    {
    //        SpawnWave();
    //        nextSpawnTime = Time.time + spawnInterval;
    //    }
    //}

    //private void SpawnWave()
    //{
    //    for (int i = 0; i < enemiesPerWave; i++)
    //    {
    //        Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;
    //        Vector3 worldPos = new Vector3(spawnPos.x, spawnPos.y, 0) + transform.position;

    //        //GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
    //        //GameObject enemy = Instantiate(enemyPrefab2, worldPos, Quaternion.identity);
    //        //GameObject enemy = Instantiate(enemyPrefab3, worldPos, Quaternion.identity);
    //        GameObject enemy = Instantiate(enemyPrefab4, worldPos, Quaternion.identity);

    //        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
    //        if (enemyBase != null)
    //            enemyBase.Initialize(playerTransform, pool);
    //    }
    //}

    public Transform[] spawnPoints;
    private List<GameObject> activeEnemies = new();
    private Coroutine spawnCoroutine;

    public void StartSpawning(WaveInfo wave)
    {
        // Sicherheitshalber alte Coroutine stoppen, falls noch aktiv
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine(wave));
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine(WaveInfo wave)
    {
        Debug.Log("Spawning enemy on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

        // läuft, solange die Wave aktiv ist
        while (FindAnyObjectByType<WaveManager>().waveActive)
        {
            // Bestimmen, wie viele Gegner in DIESEM Zyklus spawnen sollen
            int enemiesThisCycle = Random.Range(wave.minEnemies, wave.maxEnemies + 1);

            for (int i = 0; i < enemiesThisCycle; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject prefabEnemy = wave.possibleEnemies[Random.Range(0, wave.possibleEnemies.Length)];

                GameObject enemyObj = Instantiate(prefabEnemy, spawnPoint.position, Quaternion.identity);
                EnemyBase enemyBase = enemyObj.GetComponent<EnemyBase>();
                if (enemyBase != null)
                    enemyBase.Initialize(playerTransform, pool);
                activeEnemies.Add(enemyObj);
            }

            // Warte bis zum nächsten Spawn-Zyklus
            yield return new WaitForSeconds(wave.spawnRate);
        }

        Debug.Log("Spawn routine stopped — wave is over.");
    }
    internal void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if(enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}
