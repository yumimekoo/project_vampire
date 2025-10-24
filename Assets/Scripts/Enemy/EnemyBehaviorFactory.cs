using UnityEngine;

public static class EnemyBehaviorFactory
{
    public static IEnemyBehavior CreateBehavior(EnemyBehaviorType type, EnemyBase enemy, Rigidbody2D rb, Transform player, EnemyBulletPool pool)
    {
        return type switch
        {
            EnemyBehaviorType.Seek => new SeekBehavior(enemy, rb, player),
            EnemyBehaviorType.Pursue => new PursueBehavior(enemy, rb, player),
            EnemyBehaviorType.Jump => new JumpBehavior(enemy, rb, player),
            EnemyBehaviorType.Shoot => new ShootBehavior(enemy, rb, player, pool),
            _ => null,
        };
    }
    
}
