using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class PlayerStatsManager : MonoBehaviour
{
    [SerializeField] private PlayerBaseStatsSO baseStats;

    private Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> minStats = new Dictionary<StatType, float>();
    private Dictionary<StatMulti, float> currentStatMultis = new Dictionary<StatMulti, float>();

    public event System.Action OnStatChanged;
    private void Awake()
    {
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            currentStats[stat] = 0f;
        }
        InitBaseStats();
        InitMinValues();
        InitStatMulti();
        OnStatChanged?.Invoke();
    }

    private void InitStatMulti()
    {
        currentStatMultis[StatMulti.MovePercent] = 1f;
        currentStatMultis[StatMulti.AttackPercent] = 1f;
        currentStatMultis[StatMulti.DashCooldownPercent] = 1f;
        currentStatMultis[StatMulti.MaxHealthPercent] = 1f;
        currentStatMultis[StatMulti.AttackSpeedPercent] = 1f;
    }

    private void InitBaseStats()
    {
        currentStats[StatType.MoveSpeed] = baseStats.moveSpeed;
        currentStats[StatType.DashDistance] = baseStats.dashDistance;
        currentStats[StatType.DashCooldown] = baseStats.dashCD;
        currentStats[StatType.Dashes] = baseStats.dashes;
        currentStats[StatType.DashRegenerationRate] = baseStats.dashRegenerationRate;
        currentStats[StatType.MaxHealth] = baseStats.maxHealth;
        currentStats[StatType.AttackDamage] = baseStats.attackDamage;
        currentStats[StatType.AttackSpeed] = baseStats.attackSpeed;
        currentStats[StatType.BulletSpeed] = baseStats.bulletSpeed;
        currentStats[StatType.BulletDistance] = baseStats.bulletDistance;
        currentStats[StatType.BulletSpread] = baseStats.bulletSpread;
        currentStats[StatType.Defense] = baseStats.defense;
    }

    private void InitMinValues()
    {
        minStats[StatType.MoveSpeed] = 0.1f;
        minStats[StatType.DashDistance] = 0.5f;
        minStats[StatType.DashCooldown] = 0.01f;
        minStats[StatType.Dashes] = 0f;
        minStats[StatType.DashRegenerationRate] = 0f;
        minStats[StatType.MaxHealth] = 1f;
        minStats[StatType.AttackDamage] = 1f;
        minStats[StatType.AttackSpeed] = 0.1f;
        minStats[StatType.BulletSpeed] = 0.1f;
        minStats[StatType.BulletDistance] = 0.1f;
        minStats[StatType.BulletSpread] = 0f;
        minStats[StatType.Defense] = 0f;
    }

    public float GetStat(StatType stat)
    {
        return currentStats.TryGetValue(stat, out float value) ? value : 0f;
    }

    public float GetStatMulti(StatMulti stat)
    {
        return currentStatMultis.TryGetValue(stat, out float value) ? value : 1f;
    }

    internal void SetStat(StatType stat, float value)
    {
        if (currentStats.ContainsKey(stat))
            currentStats[stat] = value;
        else
            currentStats.Add(stat, value);
    }

    public void AddFlatStat(StatType stat, float value)
    {
        if(currentStats.ContainsKey(stat))
            currentStats[stat] += value;
        else 
            currentStats.Add(stat, value);

        OnStatChanged?.Invoke();
    }

    public void ReduceFlatStat(StatType stat, float value)
    {
        if (currentStats.ContainsKey(stat))
            currentStats[stat] = Mathf.Max(minStats[stat], currentStats[stat] - value);
        else
            currentStats.Add(stat, value);

        OnStatChanged?.Invoke();
    }

    public void AddPercentStat(StatMulti stat, float percent)
    {
        if(currentStatMultis.ContainsKey(stat))
            currentStatMultis[stat] += percent;
        else
            currentStatMultis.Add(stat, 1f + percent);

        OnStatChanged?.Invoke();
    }

    public void ReducePercentStat(StatMulti stat, float percent)
    {
        if(currentStatMultis.ContainsKey(stat))
            currentStatMultis[stat] = Mathf.Max(0.1f, currentStatMultis[stat] - percent);
        else
            currentStatMultis.Add(stat, 1f - percent);

        OnStatChanged?.Invoke();
    }
}
