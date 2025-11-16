using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    private PlayerStatsManager statsmanager;
    private Vector2 direction;
    private float speed;
    private float distance;
    private float damage;
    private Vector3 startPos;

    public void Init(Vector2 dir, PlayerStatsManager stats)
    {
        direction = dir.normalized;
        speed = stats.GetStat(StatType.BulletSpeed) / 10;
        distance = stats.GetStat(StatType.BulletDistance) / 10;
        damage = stats.GetStat(StatType.AttackDamage);
        statsmanager = stats;
        startPos = transform.position;
        GameEvents.OnBulletFired?.Invoke(this);
    }

    protected virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float dist = Vector3.Distance(startPos, transform.position);
        if(dist >= distance)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Obstacle"))
        {
            // implement onhit and damage
            BulletPool.Instance.ReturnBullet(gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            var data = other.GetComponent<EnemyBase>();
            data.TakeDamage(damage);
            GameEvents.OnBulletLifeSteal?.Invoke(damage);
            GameEvents.OnBulletHit?.Invoke(this, data);
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}
