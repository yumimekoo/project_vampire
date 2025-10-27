using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 250;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public static EnemyBulletPool Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < poolSize; i++) 
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.gameObject.SetActive(false);
            pool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if(pool.Count == 0)
        {
            return Instantiate(bulletPrefab, transform);
        }

        GameObject bullet = pool.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }
}
