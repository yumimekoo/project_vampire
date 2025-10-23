using UnityEngine;

public enum EnemyBehaviorType { Seek, Pursue, Jump, Shoot}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float droppedExperience;
    public float droppedMoney;
    public EnemyBehaviorType behaviorType;
}
