using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour
{
    public static BulletPoolManager Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> bulletPools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

    [Header("Pool Settings")]
    public int maxActiveBullets = 500; // Maximum number of active bullets allowed
    public int cullExcessThreshold = 100; // Number of excess bullets to keep in the pool

    private int activeBulletCount = 0;

    private void Awake()
    {
        // Ensure only one instance of the pool manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterBulletPrefab(GameObject bulletPrefab, int poolSize, Transform parent)
    {
        if (!bulletPools.ContainsKey(bulletPrefab))
        {
            // Create a child object to store the bullets
            GameObject poolParent = new GameObject($"{bulletPrefab.name}_Pool");
            poolParent.transform.SetParent(parent);

            Queue<GameObject> newPool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, poolParent.transform);
                bullet.SetActive(false);
                newPool.Enqueue(bullet);
            }

            bulletPools[bulletPrefab] = newPool;
            poolParents[bulletPrefab] = poolParent.transform;
        }
    }

    public GameObject GetBullet(GameObject bulletPrefab)
    {
        if (activeBulletCount >= maxActiveBullets)
        {
            Debug.LogWarning("Maximum active bullet limit reached. No new bullets will be spawned.");
            return null; // Prevent spawning new bullets if the limit is reached
        }

        if (bulletPools.ContainsKey(bulletPrefab) && bulletPools[bulletPrefab].Count > 0)
        {
            GameObject bullet = bulletPools[bulletPrefab].Dequeue();
            bullet.SetActive(true);
            bullet.transform.SetParent(null); // Unparent the bullet when it is retrieved
            activeBulletCount++;
            return bullet;
        }
        else
        {
            // If the pool is empty or not registered, instantiate a new bullet
            Debug.LogWarning("Pool is empty. Instantiating a new bullet.");
            GameObject bullet = Instantiate(bulletPrefab);
            activeBulletCount++;
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bulletPrefab, GameObject bullet)
    {
        bullet.SetActive(false);

        if (bulletPools.ContainsKey(bulletPrefab))
        {
            // Ensure the bullet is not already in the queue
            if (!bulletPools[bulletPrefab].Contains(bullet))
            {
                bullet.transform.SetParent(poolParents[bulletPrefab]); // Parent the bullet to the pool's child object
                bulletPools[bulletPrefab].Enqueue(bullet);
                activeBulletCount--;
            }
            else
            {
                Debug.LogWarning("Attempted to return a duplicate bullet to the pool.");
            }
        }
        else
        {
            Debug.LogWarning("Bullet prefab not registered in the pool. Destroying the bullet.");
            Destroy(bullet);
        }

        // Cull excess bullets if the pool size exceeds the threshold
        CullExcessBullets(bulletPrefab);
    }

    private void CullExcessBullets(GameObject bulletPrefab)
    {
        if (bulletPools.ContainsKey(bulletPrefab) && bulletPools[bulletPrefab].Count > cullExcessThreshold)
        {
            Queue<GameObject> pool = bulletPools[bulletPrefab];
            while (pool.Count > cullExcessThreshold)
            {
                GameObject excessBullet = pool.Dequeue();
                Destroy(excessBullet);
            }
        }
    }
}
