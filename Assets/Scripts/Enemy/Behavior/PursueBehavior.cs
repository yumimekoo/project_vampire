using UnityEngine;

public class PursueBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private Animator animator;
    private EnemyBase enemy;
    private float predictionTime = 0.4f;
    private float predictionWeight = 0.6f;

    public PursueBehavior(EnemyBase enemy,Rigidbody2D rb ,Transform player, Animator animator)
    {
        this.rb = rb;
        this.player = player;
        this.enemy = enemy;
        this.animator = animator;
    }

    public void UpdateBehavior()
    {
        if (player == null)
            return;

        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>()?.linearVelocity ?? Vector2.zero;
        Vector2 predictedPos = (Vector2)player.position + playerVelocity * predictionTime;

        Vector2 targetPos = Vector2.Lerp(player.position, predictedPos, predictionWeight);

        Vector2 direction = (targetPos - rb.position).normalized;
        rb.linearVelocity = direction * enemy.GetData().moveSpeed;
        enemy.TryAttack();

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isMoving", direction.magnitude > 0.01f);
    }
}
