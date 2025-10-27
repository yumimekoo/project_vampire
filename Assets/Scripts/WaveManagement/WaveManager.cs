using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveDataSO waveData;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject shopUI;

    private int currentWaveIndex = 0;
    private bool waveActive = false;
    private float waveTimer;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if(waveData == null || waveData.waves.Count == 0)
        {
            Debug.LogWarning("No Wave Data");
            yield break;
        }

        WaveInfo currentWave = GetCurrentWave();
        Debug.Log($"Starting waave {currentWave.waveNumber}");

        waveActive = true;
        waveTimer = currentWave.waveDuration;

        enemySpawner.StartSpawning(currentWave);

        while(waveTimer > 0f)
        {
            waveTimer -= Time.deltaTime;
            yield return null; 
        }

        waveActive = false;
        EndWave();
    }

    private void EndWave()
    {
        throw new NotImplementedException();
    }

    private WaveInfo GetCurrentWave()
    {
        throw new NotImplementedException();
    }
}
