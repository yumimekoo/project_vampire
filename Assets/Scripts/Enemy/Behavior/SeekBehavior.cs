using UnityEngine;

public class SeekBehavior : IEnemyBehavior
{
    private EnemyBase enemy;
    private Rigidbody2D rb;
    private Transform target;

    public SeekBehavior(EnemyBase enemy, Rigidbody2D rb, Transform target)
    {
        this.enemy = enemy;
        this.rb = rb;
        this.target = target;
    }

    public void UpdateBehavior()
    {
        Vector2 direction = (target.position - enemy.transform.position).normalized;
        rb.linearVelocity = direction * enemy.GetData().moveSpeed;
    }
}
