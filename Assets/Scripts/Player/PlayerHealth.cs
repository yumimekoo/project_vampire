using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerStatsManager statsManager;
    public float currentHealth;
    public float maxHealth;

    private bool isDead = false;
    private bool isInvulnerable;

    public event System.Action<float, float> OnHealthChanged;
    void Start()
    {
        if (statsManager == null)
        {
            Debug.Log("TookStatsManager");
            statsManager = GetComponent<PlayerStatsManager>();
        }

        maxHealth = statsManager.GetStat(StatType.MaxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        float defense = statsManager.GetStat(StatType.Defense);
        float finalDamage = Mathf.Max(amount - defense, 0f);

        currentHealth -= finalDamage;
        Debug.Log($"Player took {finalDamage} damage ({ currentHealth}/{ maxHealth})");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if(currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth+amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Healed {amount}. Current HP: {currentHealth}/{maxHealth}");
    }

    public void Die()
    {
        isDead = true;
        currentHealth = 0f;

        Debug.Log("Player DIED!");
        // Death screen implement
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    internal void HealToFull()
    {
        Debug.Log("Healed to Full");
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        //UpdateHealthUI();
    }
}
