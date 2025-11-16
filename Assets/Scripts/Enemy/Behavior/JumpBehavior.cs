using UnityEngine;

public class JumpBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private EnemyBase enemy;

    private float jumpTimer;
    private bool isJumping = false;
    private float jumpDuration = 0.4f; // wie lange der Sprung andauert
    private float jumpTimeRemaining;
    private float jumpCooldown = 3f;
    private float jumpForce = 18f;

    private Animator animator;
    private float chargeTime = 1f;
    private bool isCharging = false;
    private float chargeTimeRemaining;

    public JumpBehavior(EnemyBase enemy, Rigidbody2D rb, Transform player, Animator animator)
    {
        this.enemy = enemy;
        this.rb = rb;
        this.player = player;
        this.animator = animator;
        jumpTimer = Random.Range(1f, 3f);
    }

    public void UpdateBehavior()
    {
        if (player == null)
            return;

        if (isCharging || isJumping)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 0f);
        }

        jumpTimer -= Time.deltaTime;

        if (isJumping)
        {
            jumpTimeRemaining -= Time.deltaTime;
            if (jumpTimeRemaining <= 0f)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
                rb.linearVelocity = Vector2.zero; // bremst nach dem Sprung ab
            }
            return; // während des Sprungs nichts anderes machen
        }

        if (isCharging)
        {
            chargeTimeRemaining -= Time.deltaTime;
            if (chargeTimeRemaining <= 0f)
            {
                isCharging = false;
                JumpTowardsPlayer();
            }
            return; // während des Aufladens nichts anderes machen
        }

        if (jumpTimer <= 0f && !isCharging && !isJumping)
        {
            StartCharging();
            jumpTimer = jumpCooldown;
        }
        else
        {
            // Langsames „Heranschleichen“
            Vector2 dir = ((Vector2) player.position - rb.position).normalized;
            rb.linearVelocity = dir * (enemy.GetData().moveSpeed * 0.3f);

            animator.SetFloat("moveX", dir.x);
            animator.SetFloat("moveY", dir.y);
            animator.SetBool("isWalking", dir.magnitude > 0.01f);
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeTimeRemaining = chargeTime;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Charge"); 
    }

    private void JumpTowardsPlayer()
    {
        isJumping = true;
        animator.SetBool("isJumping", true);
        jumpTimeRemaining = jumpDuration;

        Vector2 jumpDir = ((Vector2) player.position - rb.position).normalized;
        rb.linearVelocity = jumpDir * jumpForce;
    }

}
