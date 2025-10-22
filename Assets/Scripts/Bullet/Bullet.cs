using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float distance;
    private float damage;
    private Vector3 startPos;

    public void Init(Vector2 dir, PlayerStatsManager stats)
    {
        direction = dir.normalized;
        speed = stats.GetStat(StatType.BulletSpeed);
        distance = stats.GetStat(StatType.BulletDistance);
        damage = stats.GetStat(StatType.AttackDamage);
        startPos = transform.position;
    }
 
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float dist = Vector3.Distance(startPos, transform.position);
        if(dist >= distance)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Obstacle"))
        {
            // implement onhit and damage
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}
