
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyBulletPool pool;
    [SerializeField] private LevelManager levelManager;

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

        // l�uft, solange die Wave aktiv ist
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
                    enemyBase.Initialize(playerTransform, pool, levelManager);
                activeEnemies.Add(enemyObj);
            }

            // Warte bis zum n�chsten Spawn-Zyklus
            yield return new WaitForSeconds(wave.spawnRate);
        }

        Debug.Log("Spawn routine stopped � wave is over.");
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
