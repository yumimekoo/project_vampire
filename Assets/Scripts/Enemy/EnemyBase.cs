using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    
    private Transform player;
    private IEnemyBehavior behavior;
    private Rigidbody2D rb;
    private LevelManager levelManager;
    public float Health {  get; private set; }
    public float lastAttackTime = -999f;

    public void Initialize(Transform playerTransform, EnemyBulletPool pool, LevelManager level)
    {
        player = playerTransform;
        levelManager = level;
        behavior = EnemyBehaviorFactory.CreateBehavior(data.behaviorType, this, rb, player.transform, pool);
        Health = data.maxHealth * levelManager.GetEnemyDifficultyMultiplier();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    public void Update()
    {
        behavior?.UpdateBehavior();
    }

    public bool TryAttack()
    {
        if (Time.time < lastAttackTime + data.attackCooldown)
            return false;

        lastAttackTime = Time.time;

        if (player==null) return false;

        float distance = Vector2.Distance(transform.position, player.position);

        if(distance <= data.attackRange)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(data.attackDamage * levelManager.GetEnemyDifficultyMultiplier());
            return true;
        }

        return false;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        // TODO: Add death effects, pooling usw. experience to player
        levelManager.AddXP(data.droppedExperience * levelManager.GetEnemyDifficultyMultiplier());
        levelManager.AddScore((int) (data.droppedExperience * levelManager.GetEnemyDifficultyMultiplier() * levelManager.GetLevel()));
        levelManager.AddMoney((int)(data.droppedMoney * levelManager.GetEnemyDifficultyMultiplier()));
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!data.isDealingContactDamage) return;

        if (other.CompareTag("Player") && Time.time >= lastAttackTime + data.attackCooldown)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(data.attackDamage);
                lastAttackTime = Time.time;
            }
        }

    }

    public EnemyData GetData() => data;
}
