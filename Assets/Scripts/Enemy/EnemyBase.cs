using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip shootSound;

    private EnemyHealthBar healthBarInstance;

    private Transform player;
    private IEnemyBehavior behavior;
    private Rigidbody2D rb;
    private LevelManager levelManager;
    private AudioSource audioSource;
    public float Health {  get; private set; }
    public float maxHealthMultiplied {  get; private set; }
    public float lastAttackTime = -999f;

    public void Initialize(Transform playerTransform, EnemyBulletPool pool, LevelManager level, AudioSource audio)
    {
        audioSource = audio;
        player = playerTransform;
        levelManager = level;
        behavior = EnemyBehaviorFactory.CreateBehavior(data.behaviorType, this, rb, player.transform, pool, animator);
        Health = data.maxHealth * levelManager.GetEnemyDifficultyMultiplier();
        maxHealthMultiplied = data.maxHealth * levelManager.GetEnemyDifficultyMultiplier();
    }

    public void SetHealthBar(EnemyHealthBar bar)
    {
        healthBarInstance = bar;
        bar.Initialize(transform);
        Debug.Log("Setting health bar: " + maxHealthMultiplied + "/" + maxHealthMultiplied);
        bar.UpdateHealthBar(maxHealthMultiplied, maxHealthMultiplied);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    public void Update()
    {
        behavior?.UpdateBehavior();
    }

    public void PlayShootSound() { 
        audioSource.PlayOneShot(shootSound);
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
            audioSource.PlayOneShot(attackSound);
            player.GetComponent<PlayerHealth>()?.TakeDamage(data.attackDamage * levelManager.GetEnemyDifficultyMultiplier());
            return true;
        }

        return false;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Health = Mathf.Max(Health, 0);
        healthBarInstance?.UpdateHealthBar(Health, maxHealthMultiplied);
        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        audioSource.PlayOneShot(deathSound);
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        GameEvents.OnEnemyKilled?.Invoke(this);
        // TODO: Add death effects, pooling usw. experience to player
        levelManager.AddXP(data.droppedExperience * levelManager.GetEnemyDifficultyMultiplier());
        levelManager.AddScore((int) (data.droppedExperience * levelManager.GetEnemyDifficultyMultiplier() * levelManager.GetLevel()));
        levelManager.AddMoney((int)(data.droppedMoney * levelManager.GetEnemyDifficultyMultiplier()) * 2);
        Destroy(gameObject);
        Destroy(healthBarInstance.gameObject);
        
    }

    public void Explode()
    {
        Debug.Log("Enemy exploded");
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
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
