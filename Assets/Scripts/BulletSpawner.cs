using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    enum SpawnerType { Random, Spin, Straight, Fan, Wave, Burst, Spiral, Arc, Explosion }

    [Header("Bullet Prefab")]
    public GameObject bulletPrefab;
    public float bulletLife;
    public float speed;

    [Header("Spawn Attributes")]
    [SerializeField] private List<SpawnerType> activePatterns = new List<SpawnerType>(); // List of active patterns
    [SerializeField] private float firingRate = 1f;
    [SerializeField] private int bulletCount = 5; // Number of bullets for patterns like Fan, Burst, or Explosion
    [SerializeField] private float spreadAngle = 45f; // Spread angle for Fan or Arc patterns
    [SerializeField] private float waveFrequency = 1f; // Frequency for Wave pattern
    [SerializeField] private float waveAmplitude = 1f; // Amplitude for Wave pattern
    [SerializeField] private float spiralRotationSpeed = 10f; // Speed of rotation for Spiral pattern

    [Header("Rotation Settings")]
    public bool rotateSpawner = false; // Enable/disable spawner rotation
    public float rotationSpeed = 1f; // Speed of spawner rotation

    [Header("Targeting")]
    public Transform target; // Target to aim at (e.g., the player)
    public bool targetPlayer = false; // Enable targeting for all patterns

    private float timer = 0f;

    void FixedUpdate()
    {
        timer += Time.deltaTime;

        // Rotate the spawner if enabled
        if (rotateSpawner)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        if (timer >= firingRate)
        {
            FireAllPatterns();
            timer = 0f;
        }
    }

    private void FireAllPatterns()
    {
        if (!bulletPrefab) return;

        // If targeting is enabled, aim at the target
        if (targetPlayer && target != null)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Iterate through all active patterns and fire bullets for each
        foreach (SpawnerType pattern in activePatterns)
        {
            switch (pattern)
            {
                case SpawnerType.Straight:
                    SpawnBullet(transform.rotation);
                    break;

                case SpawnerType.Fan:
                    FireFanPattern();
                    break;

                case SpawnerType.Wave:
                    FireWavePattern();
                    break;

                case SpawnerType.Burst:
                    FireBurstPattern();
                    break;

                case SpawnerType.Random:
                    FireRandomPattern();
                    break;

                case SpawnerType.Spin:
                    FireSpinPattern();
                    break;

                case SpawnerType.Spiral:
                    FireSpiralPattern();
                    break;

                case SpawnerType.Arc:
                    FireArcPattern();
                    break;

                case SpawnerType.Explosion:
                    FireExplosionPattern();
                    break;
            }
        }
    }

    private void SpawnBullet(Quaternion rotation)
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab, transform.position, rotation);
        Bullet bullet = spawnedBullet.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.bulletLife = bulletLife;
            bullet.speed = speed;
            bullet.isPlayerBullet = false;
            bullet.hasHitPlayer = false;
            bullet.hasBeenGrazed = false;

            // Enable homing and assign the target
            if (targetPlayer && target != null)
            {
                bullet.isHoming = true;
                bullet.target = target;
                bullet.homingSpeed = 2f;
            }
        }
    }

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
        float baseAngle = transform.eulerAngles.z; 
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + waveOffset);
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
        float randomAngle = transform.eulerAngles.z + Random.Range(-180f, 180f);
        Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
        SpawnBullet(rotation);
    }

    private void FireSpinPattern()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        SpawnBullet(transform.rotation);
    }
}
