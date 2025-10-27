using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataSO", menuName = "Scriptable Objects/WaveDataSO")]
public class WaveDataSO : ScriptableObject
{
    public List<WaveInfo> waves;
}

[System.Serializable]
public class WaveInfo
{
    public int waveNumber;
    public float waveDuration;
    public int minEnemys;
    public int maxEnemys;
    public float spawnRate;
    public EnemyData[] possibleEnemies;
}
