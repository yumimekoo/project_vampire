using UnityEngine;

public static class EnemyBehaviorFactory
{
    public static IEnemyBehavior CreateBehavior(EnemyBehaviorType type, EnemyBase enemy, Rigidbody2D rb, Transform player)
    {
        return type switch
        {
            EnemyBehaviorType.Seek => new SeekBehavior(enemy, rb, player),
            _ => null,
        };
    }
    
}
