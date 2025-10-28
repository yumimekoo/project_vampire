using UnityEngine;

public class ShootBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private readonly EnemyBase enemy;
    private readonly EnemyBulletPool pool;
    private readonly float safeRange = 3f;
    //private readonly float shootCooldown = 2f;
    private float shootTimer;
    private bool isShooting;
    private float shootTimerCurrent;
    private float shootDuration = 1f;

    public ShootBehavior(EnemyBase enemy, Rigidbody2D rb, Transform player, EnemyBulletPool pool)
    {
        this.rb = rb;
        this.player = player;
        this.enemy = enemy;
        shootTimer = Random.Range(0f, enemy.GetData().attackCooldown); // damit nicht alle gleichzeitig schieﬂen
        this.pool = pool;
    }

    public void UpdateBehavior()
    {
        if (player == null)
            return;

        Vector2 dirToPlayer = (player.position - rb.transform.position);
        float distance = dirToPlayer.magnitude;
        Vector2 moveDir = dirToPlayer.normalized;

        if (isShooting)
        {
            rb.linearVelocity = Vector2.zero; // Gegner bleibt w‰hrend Schuss stehen
            shootTimerCurrent -= Time.deltaTime;
            if (shootTimerCurrent <= 0f)
            {
                isShooting = false;
            }
            return; // w‰hrend Shooting keine Bewegung
        }

        shootTimer -= Time.deltaTime;
        
        if (shootTimer <= 0f && distance <= enemy.GetData().attackRange)
        {
            ShootAtPlayer();
            shootTimer = enemy.GetData().attackCooldown;
            isShooting = true;
            shootTimerCurrent = shootDuration;
            return; // Gegner bleibt kurz stehen
        }

        // Bewegung
        if (distance > enemy.GetData().attackRange)
        {
            // zu weit weg -> n‰her kommen
            rb.linearVelocity = moveDir * enemy.GetData().moveSpeed;
        }
        else if (distance < safeRange)
        {
            // zu nah -> zur¸ckweichen
            rb.linearVelocity = -moveDir * enemy.GetData().moveSpeed * 1.2f;
        }
        else
        {
            // optimale Schussdistanz -> stehen bleiben und schieﬂen
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ShootAtPlayer()
    {
        GameObject bullet = pool.GetBullet();
        bullet.transform.position = rb.position;
        bullet.GetComponent<EnemyBullet>().InitEnemy(player ,enemy);

        Debug.Log($"{enemy.name} fires at player!");
    }
}

