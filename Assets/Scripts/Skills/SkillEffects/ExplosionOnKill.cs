using UnityEngine;

public class ExplosionOnKillEffect : ISkillEffect
{
    public string EffectName => "ExplosionOnKill";

    private float radius;
    private float damage;
    private float cooldown;

    private float lastTriggerTime;

    public ExplosionOnKillEffect(float radius, float damage, float cooldown)
    {
        this.radius = radius;
        this.damage = damage;
        this.cooldown = cooldown;
    }

    public void Register()
    {
        GameEvents.OnEnemyKilled += OnEnemyKilled;
    }

    public void Unregister()
    {
        GameEvents.OnEnemyKilled -= OnEnemyKilled;
    }

    public void AddStack(float extraRadius, float extraDamage)
    {
        radius += extraRadius;   // z. B. Explosion wird größer
        damage += extraDamage;   // z. B. Schaden wird stärker
    }

    private void OnEnemyKilled(EnemyBase enemy)
    {
        if (enemy == null)
            return;

        // Cooldown-Check, genau wie BleedOnHit
        if (Time.time < lastTriggerTime + cooldown)
            return;

        lastTriggerTime = Time.time;

        Vector3 pos = enemy.transform.position;

        enemy.Explode(); // Visueller Effekt der Explosion

        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        // Explosion Damage im Radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, enemyLayer);
        foreach (var hit in hits)
        {
            Debug.Log($"ExplosionOnKillEffect hits: {hit.name}");
            if (hit.TryGetComponent<EnemyBase>(out EnemyBase e))
            {
                Debug.Log($"ExplosionOnKillEffect damages: {e.name}");
                e.TakeDamage(damage);
            }
        }
    }
}