using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] PlayerStatsManager statsManager;
    [SerializeField] PlayerMovementController movement;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip healSound;
    [SerializeField] AudioClip blockedSound;

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
        GameEvents.OnBulletLifeSteal += ApplyLifeSteal;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        if (Time.time < lastHitTime + hitCooldown)
            return;

        float defense = statsManager.GetStat(StatType.Defense);
        float finalDamage = Mathf.Max(amount - defense, 0f);

        if (finalDamage < 1f)
        {
            audioSource.PlayOneShot(blockedSound);
        } else
        {
            currentHealth -= finalDamage;
            Debug.Log($"Player took {finalDamage} damage ({currentHealth}/{maxHealth})");

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            audioSource.PlayOneShot(hitSound);
            if (currentHealth <= 0f)
            {
                Die();
            }
            GameEvents.OnPlayerHit?.Invoke(this, finalDamage);
        }
    }

    public void ApplyLifeSteal(float damageDealt)
    {
        float lifeStealPercent = statsManager.GetStatMulti(StatMulti.LifeSteal);
        float healAmount = damageDealt * lifeStealPercent;
        HealLifeSteal(healAmount);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        audioSource.PlayOneShot(healSound);
        currentHealth = Mathf.Min(currentHealth+amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Healed {amount}. Current HP: {currentHealth}/{maxHealth}");
    }

    public void HealLifeSteal(float amount)
    {
        if (isDead)
            return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
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
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    internal void HealToFull()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void UpdateMaxHealth()
    {
        maxHealth = statsManager.GetStat(StatType.MaxHealth) * statsManager.GetStatMulti(StatMulti.MaxHealthPercent);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
