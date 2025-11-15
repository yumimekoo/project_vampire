using UnityEngine;

public class ShootBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private readonly EnemyBase enemy;
    private readonly EnemyBulletPool pool;
    private Animator animator;
    private readonly float safeRange = 3f;
    //private readonly float shootCooldown = 2f;
    private float shootTimer;
    private bool isShooting;
    private bool hasFired;
    private float shootWindup = 0.25f;
    private float shootTimerCurrent;
    private float shootDuration = 1f;
    private float cachedShootDirX = 1f;

    public ShootBehavior(EnemyBase enemy, Rigidbody2D rb, Transform player, EnemyBulletPool pool, Animator animator)
    {
        this.rb = rb;
        this.player = player;
        this.enemy = enemy;
        shootTimer = Random.Range(0f, enemy.GetData().attackCooldown); // damit nicht alle gleichzeitig schieﬂen
        this.pool = pool;
        this.animator = animator;
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
            Vector2 shootDir = (player.position - rb.transform.position).normalized;
        

            shootTimerCurrent -= Time.deltaTime;
            //if (shootTimerCurrent <= 0f)
            //{
            //    isShooting = false;
            //    animator.SetBool("isShooting", false);
            //}
            //return; // w‰hrend Shooting keine Bewegung
            if (!hasFired && shootTimerCurrent <= (shootDuration - shootWindup))
            {
                ShootAtPlayer();
                hasFired = true;
            }
            if (shootTimerCurrent <= 0f)
            {
                isShooting = false;
                hasFired = false;
                animator.SetBool("isShooting", false);
            }
            return;
        }

        shootTimer -= Time.deltaTime;

        //if (shootTimer <= 0f && distance <= enemy.GetData().attackRange)
        //{
        //    ShootAtPlayer();
        //    shootTimer = enemy.GetData().attackCooldown;
        //    isShooting = true;
        //    shootTimerCurrent = shootDuration;
        //    return; // Gegner bleibt kurz stehen
        //}

        if (shootTimer <= 0f && distance <= enemy.GetData().attackRange)
        {
            BeginShooting();
            shootTimer = enemy.GetData().attackCooldown; // setze cooldown hier, damit es nicht sofort wieder startet
            return;
        }

        // Bewegung
        if (distance > enemy.GetData().attackRange)
        {
            // zu weit weg -> n‰her kommen
            rb.linearVelocity = moveDir * enemy.GetData().moveSpeed;
            animator.SetFloat("moveX", moveDir.x);
            animator.SetFloat("moveY", moveDir.y);
        }
        else if (distance < safeRange)
        {
            // zu nah -> zur¸ckweichen
            rb.linearVelocity = -moveDir * enemy.GetData().moveSpeed * 1.2f;
            animator.SetFloat("moveX", -moveDir.x);
            animator.SetFloat("moveY", -moveDir.y);
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
        Vector2 dir = (player.position - rb.transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        bullet.GetComponent<EnemyBullet>().InitEnemy(player ,enemy);
        Debug.Log($"{enemy.name} fires at player!");
    }

    private void BeginShooting()
    {
        isShooting = true;
        hasFired = false;
        shootTimerCurrent = shootDuration;

        // Setze Animator f¸r Shooting und die Richtung sofort, damit Windup-Animation startet
        cachedShootDirX = (player.position.x - rb.transform.position.x) >= 0f ? 1f : -1f;
        animator.SetFloat("shootDirectionX", cachedShootDirX);
        animator.SetBool("isShooting", true);
        

        // Optional: falls du eine spezielle Auflade-Animation vor Schuss willst, kannst du hier einen Trigger setzen:
        // animator.SetTrigger("StartShootWindup");
    }
}

