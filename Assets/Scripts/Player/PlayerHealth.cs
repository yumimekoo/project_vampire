using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerStatsManager statsManager;
    [SerializeField] PlayerMovementController movement;
    public float currentHealth;
    public float maxHealth;

    private bool isDead = false;
    private bool isInvulnerable;

    private float lastHitTime = -999f;
    public float hitCooldown = 1f;

    public event System.Action<float, float> OnHealthChanged;
    public event System.Action OnPlayerDeath;
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

        if (Time.time < lastHitTime + hitCooldown)
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
        GameEvents.OnPlayerHit?.Invoke(this, finalDamage);
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
        GameState.isDead = true;
        isDead = true;
        currentHealth = 0f;
        movement.VelocityRemove();
        OnPlayerDeath?.Invoke();
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

    public void UpdateMaxHealth()
    {
        maxHealth = statsManager.GetStat(StatType.MaxHealth) * statsManager.GetStatMulti(StatMulti.MaxHealthPercent);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
