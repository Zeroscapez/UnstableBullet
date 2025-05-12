using System.Collections.Generic;
using UnityEngine;

public enum SpawnerType { Random, Spin, Straight, Fan, Wave, Burst, Spiral, Arc, Explosion }

public class BulletSpawner : MonoBehaviour
{
    [Header("Bullet Prefab")]
    public GameObject bulletPrefab;
    public float bulletLife = 5f;
    public float speed;

    [Header("Spawn Attributes")]
    [SerializeField] public List<SpawnerType> activePatterns = new List<SpawnerType>();
    [SerializeField] public float firingRate = 1f;
    [SerializeField] public int bulletCount = 5;
    [SerializeField] public float spreadAngle = 45f;
    [SerializeField] public float waveFrequency = 1f;
    [SerializeField] public float waveAmplitude = 1f;
    [SerializeField] public float spiralRotationSpeed = 10f;

    [Header("Rotation Settings")]
    public bool rotateSpawner = false;
    public float rotationSpeed = 1f;

    [Header("Targeting")]
    public Transform target;

    [Header("Homing Settings")]
    public bool enableHoming = false;

    private float timer = 0f;

    private Queue<GameObject> bulletPool;
    public int poolSize;
    public int maxPoolSize = 100;
    public Transform bulletOwner;

    private void Start()
    {
        target = LevelManager.Instance.playerController.transform;
        bulletOwner = this.transform;
        poolSize = bulletCount * 2;
        InitializePool();
    }

    private void InitializePool()
    {
        bulletPool = new Queue<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullet.GetComponent<BulletBase>().poolOwner = this;
            bulletPool.Enqueue(bullet);
        }
    }

    private GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            // Optionally expand the pool if needed
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<BulletBase>().poolOwner = this;
            return bullet;
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (rotateSpawner)
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (timer >= firingRate)
        {
            FireAllPatterns();
            timer = 0f;
        }
    }

    private void FireAllPatterns()
    {
        if (!bulletPrefab) return;

        foreach (SpawnerType pattern in activePatterns)
        {
            switch (pattern)
            {
                case SpawnerType.Straight: SpawnBullet(transform.rotation); break;
                case SpawnerType.Fan: FireFanPattern(); break;
                case SpawnerType.Wave: FireWavePattern(); break;
                case SpawnerType.Burst: FireBurstPattern(); break;
                case SpawnerType.Random: FireRandomPattern(); break;
                case SpawnerType.Spin: FireSpinPattern(); break;
                case SpawnerType.Spiral: FireSpiralPattern(); break;
                case SpawnerType.Arc: FireArcPattern(); break;
                case SpawnerType.Explosion: FireExplosionPattern(); break;
            }
        }
    }





    // Update your SpawnBullet method:
    private void SpawnBullet(Quaternion rotation)
    {
        GameObject bullet = GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = rotation;

        BulletBase bulletScript = bullet.GetComponent<BulletBase>();
        if (bulletScript != null)
        {
            bulletScript.speed = speed;
            bulletScript.bulletLife = bulletLife;
            bulletScript.poolOwner = this;
            bulletScript.isHoming = enableHoming;
            bulletScript.target = target;
            
        }
    }

    // Pattern Implementations
    private void FireFanPattern()
    {
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);
            SpawnBullet(rotation);
        }
    }

    private void FireWavePattern()
    {
        float offset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + offset);
        SpawnBullet(rotation);
    }

    private void FireBurstPattern()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(transform.rotation);
        }
    }

    private void FireSpiralPattern()
    {
        transform.Rotate(0, 0, spiralRotationSpeed * Time.deltaTime);
        SpawnBullet(transform.rotation);
    }

    private void FireArcPattern()
    {
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);
            SpawnBullet(rotation);
        }
    }

    private void FireExplosionPattern()
    {
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = transform.eulerAngles.z + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            SpawnBullet(rotation);
        }
    }

    private void FireRandomPattern()
    {
        float angle = transform.eulerAngles.z + Random.Range(-180f, 180f);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        SpawnBullet(rotation);
    }

    private void FireSpinPattern()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        SpawnBullet(transform.rotation);
    }
}
