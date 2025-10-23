using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    
    private Transform player;
    private IEnemyBehavior behavior;
    private Rigidbody2D rb;
    

    public float Health {  get; private set; }


    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        behavior = EnemyBehaviorFactory.CreateBehavior(data.behaviorType, this, rb, player.transform);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Health = data.maxHealth;

    }

    public void Update()
    {
        behavior?.UpdateBehavior();
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
        Destroy(gameObject);
    }

    public EnemyData GetData() => data;
}
