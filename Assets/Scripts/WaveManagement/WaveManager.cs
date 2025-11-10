using System;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveDataSO waveData;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private PlayerMovementController playerMovementController;

    public int currentWaveIndex = 0;
    public bool waveActive;
    private float waveTimer;

    public event System.Action OnWaveChanged;
    void Start()
    {
        waveActive = false;
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

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, waveTimer);
    }

    private void EndWave()
    {
        Debug.Log("Wave ended.");
        waveActive = false;
        enemySpawner.StopSpawning();
        enemySpawner.ClearAllEnemies();
        playerMovementController.SetToZero();
        shopUI.ShowUI();
    }

    public void OnNextWaveButton()
    {
        playerMovementController.ResetDashes();
        playerMovementController.SetToZero();
        shopManager.RollItems();
        playerHealth.HealToFull();
        currentWaveIndex++;
        OnWaveChanged?.Invoke();
        StartCoroutine(StartNextWave());
    }

    private WaveInfo GetCurrentWave()
    {
        if (currentWaveIndex < waveData.waves.Count)
            return waveData.waves[currentWaveIndex];
        else
            return GenerateEndlessWave(currentWaveIndex);
    }

    private WaveInfo GenerateEndlessWave(int waveNumber)
    {
        WaveInfo newWave = new WaveInfo
        {
            waveNumber = waveNumber + 1,
            waveDuration = 30f + waveNumber * 2f,
            minEnemies = 3 + waveNumber,
            maxEnemies = 6 + waveNumber * 2,
            spawnRate = Mathf.Max(7f, 14f - waveNumber * 0.1f),
            possibleEnemies = waveData.waves[^1].possibleEnemies,
        };
        return newWave;
    }
}
