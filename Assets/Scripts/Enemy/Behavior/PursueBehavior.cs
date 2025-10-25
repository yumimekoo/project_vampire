using UnityEngine;

public class PursueBehavior : IEnemyBehavior
{
    private readonly Rigidbody2D rb;
    private readonly Transform player;
    private EnemyBase enemy;
    private float predictionTime = 0.4f;
    private float predictionWeight = 0.6f;

    public PursueBehavior(EnemyBase enemy,Rigidbody2D rb ,Transform player)
    {
        this.rb = rb;
        this.player = player;
        this.enemy = enemy;
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
    }
}
