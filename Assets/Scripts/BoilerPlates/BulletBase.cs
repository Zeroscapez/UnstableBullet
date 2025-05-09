using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public float bulletLife = 1f; // Time before bullet is destroyed    
    public float speed;
    public bool isHoming = false;
    public float homingSpeed = 2f; // Speed at which the bullet adjusts its direction
    public Transform target; // Target to home onto (e.g., the player)

    [HideInInspector]
    public GameObject bulletPrefab; // Reference to the prefab this bullet belongs to

    private float timer = 0f;

    protected virtual void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (isHoming && target != null)
        {
            HomeOntoTarget();
        }
        else
        {
            MoveStraight();
        }

        CheckBounds();
    }

    private void MoveStraight()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void HomeOntoTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingSpeed * Time.deltaTime);
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void CheckBounds()
    {
        if (transform.position.x < PlayAreaManager.Instance.MinX - 50 ||
            transform.position.x > PlayAreaManager.Instance.MaxX + 50 ||
            transform.position.y < PlayAreaManager.Instance.MinY - 50 ||
            transform.position.y > PlayAreaManager.Instance.MaxY + 50)
        {
            ReturnToPool();
        }
    }

    protected abstract void OnTriggerEnter2D(Collider2D other);

    protected void ReturnToPool()
    {
        if (bulletPrefab != null)
        {
            BulletPoolManager.Instance.ReturnBullet(bulletPrefab, gameObject);
        }
        else
        {
            Debug.LogWarning("Bullet prefab reference is missing. Destroying the bullet.");
            Destroy(gameObject);
        }
    }
}
