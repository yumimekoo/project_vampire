using UnityEngine;

public class BulletInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerStatsManager statsManager;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    //private float attackTimer = 0f;
    private float nextShootTime = 0f;
    void Update()
    {

        //if (Input.GetButtonDown("Fire1"))
        //{
        //    nextShootTime = Time.time;
        //}

        //attackTimer -= Time.deltaTime;
        if (Input.GetButton("Fire1") && Time.time >= nextShootTime) 
        {
            Shoot();

            float attackSpeed = statsManager.GetStat(StatType.AttackSpeed);
            nextShootTime = Time.time + 1f / attackSpeed;
        }

        //if (Input.GetButtonUp("Fire1"))
        //{
        //    nextShootTime = 0f;
        //}
    }


//private void TryShoot()
//{
//    float attackSpeed = statsManager.GetStat(StatType.AttackSpeed);
//    float attackCooldown = 1f/ attackSpeed;

//    if (attackTimer <= 0f)
//    {
//        Shoot();
//        attackTimer = attackCooldown;

//    }   
//}

private void Shoot()
    {
        float spread = statsManager.GetStat(StatType.BulletSpread);
        float spreadAngle = Random.Range(-spread, spread);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        direction = Quaternion.Euler(0, 0, spreadAngle) * direction;

        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.GetComponent<Bullet>().Init(direction, statsManager);
    }

}
