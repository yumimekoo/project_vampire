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
        if(!GameState.inPauseMenu && !GameState.inTabPauseMenu && !GameState.inShop)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
            {
                Shoot();

                float attackSpeed = statsManager.GetStat(StatType.AttackSpeed) / 100;
                nextShootTime = Time.time + 1f / attackSpeed;
            }
        }

    }



private void Shoot()
    {
        float spread = statsManager.GetStat(StatType.BulletSpread);
        float spreadAngle = Random.Range(-spread, spread);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        direction = Quaternion.Euler(0, 0, spreadAngle) * direction;

        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = firePoint.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        bullet.GetComponent<Bullet>().Init(direction, statsManager);
    }

}
