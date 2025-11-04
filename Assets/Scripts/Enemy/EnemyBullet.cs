using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Vector2 directionEnemy;
    private float speedEnemy;
    private float distanceEnemy;
    private float damageEnemy;
    private Vector3 startPosEnemy;
    public void InitEnemy(Transform player ,EnemyBase enemy)
    {
        directionEnemy = ((Vector2) player.position - (Vector2) enemy.transform.position).normalized;
        speedEnemy = 6f; // LATER MODULAR MACHEN stats.GetStat(StatType.BulletSpeed);
        distanceEnemy = 5f; // LATER MODULAR mACHEN stats.GetStat(StatType.BulletDistance);
        damageEnemy = enemy.GetData().attackDamage;
        startPosEnemy = transform.position;
    }

    private void Update()
    {
        transform.Translate(directionEnemy * speedEnemy * Time.deltaTime, Space.World);

        float dist = Vector3.Distance(startPosEnemy, transform.position);
        if (dist >= distanceEnemy)
        {
            EnemyBulletPool.Instance.ReturnBullet(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            EnemyBulletPool.Instance.ReturnBullet(gameObject);
            other.GetComponent<PlayerHealth>()?.TakeDamage(damageEnemy);
        }

        if (other.CompareTag("Obstacle"))
        {
            EnemyBulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}
