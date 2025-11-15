using UnityEngine;

public class SeekBehavior : IEnemyBehavior
{
    private EnemyBase enemy;
    private Rigidbody2D rb;
    private Transform target;
    private Animator animator;

    public SeekBehavior(EnemyBase enemy, Rigidbody2D rb, Transform target, Animator animator)
    {
        this.enemy = enemy;
        this.rb = rb;
        this.target = target;
        this.animator = animator;
    }

    public void UpdateBehavior()
    {
        Vector2 direction = (target.position - enemy.transform.position).normalized;
        rb.linearVelocity = direction * enemy.GetData().moveSpeed;
        enemy.TryAttack();

        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        animator.SetBool("isMoving", direction.magnitude > 0.01f);
    }
}
