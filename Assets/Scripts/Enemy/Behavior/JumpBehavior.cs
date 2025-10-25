using UnityEngine;

public class JumpBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private EnemyBase enemy;

    private float jumpTimer;
    private bool isJumping = false;
    private float jumpDuration = 0.3f; // wie lange der Sprung andauert
    private float jumpTimeRemaining;
    private float jumpCooldown = 3f;
    private float jumpForce = 15f;

    public JumpBehavior(EnemyBase enemy, Rigidbody2D rb, Transform player)
    {
        this.enemy = enemy;
        this.rb = rb;
        this.player = player;
        jumpTimer = Random.Range(1f, 3f);
    }

    public void UpdateBehavior()
    {
        if (player == null)
            return;

        jumpTimer -= Time.deltaTime;

        if (isJumping)
        {
            jumpTimeRemaining -= Time.deltaTime;
            if (jumpTimeRemaining <= 0f)
            {
                isJumping = false;
                rb.linearVelocity = Vector2.zero; // bremst nach dem Sprung ab
            }
            return; // während des Sprungs nichts anderes machen
        }

        if (jumpTimer <= 0f)
        {
            JumpTowardsPlayer();
            jumpTimer = jumpCooldown;
        }
        else
        {
            // Langsames „Heranschleichen“
            Vector2 dir = ((Vector2) player.position - rb.position).normalized;
            rb.linearVelocity = dir * (enemy.GetData().moveSpeed * 0.3f);
        }
    }

    private void JumpTowardsPlayer()
    {
        isJumping = true;
        jumpTimeRemaining = jumpDuration;

        Vector2 jumpDir = ((Vector2) player.position - rb.position).normalized;
        rb.linearVelocity = jumpDir * jumpForce;
    }

}
