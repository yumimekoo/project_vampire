using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerStatsManager : MonoBehaviour
{
    [SerializeField] private PlayerBaseStatsSO baseStats;

    private Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();

    private void Awake()
    {
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            currentStats[stat] = 0f;
        }
        InitBaseStats();
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
    }

    public float GetStat(StatType stat)
    {
        return currentStats.TryGetValue(stat, out float value) ? value : 0f;
    }

    internal void SetStat(StatType stat, float value)
    {
        if (currentStats.ContainsKey(stat))
            currentStats[stat] = value;
        else
            currentStats.Add(stat, value);
    }
}
