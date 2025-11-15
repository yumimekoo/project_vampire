using UnityEngine;

public static class EnemyBehaviorFactory
{
    public static IEnemyBehavior CreateBehavior(EnemyBehaviorType type, EnemyBase enemy, Rigidbody2D rb, Transform player, EnemyBulletPool pool, Animator animator)
    {
        return type switch
        {
            EnemyBehaviorType.Seek => new SeekBehavior(enemy, rb, player, animator),
            EnemyBehaviorType.Pursue => new PursueBehavior(enemy, rb, player, animator),
            EnemyBehaviorType.Jump => new JumpBehavior(enemy, rb, player, animator),
            EnemyBehaviorType.Shoot => new ShootBehavior(enemy, rb, player, pool, animator),
            _ => null,
        };
    }
    
}
