using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Anything that owns a bullet pool (BulletSpawner, PlayerController)
public interface IBulletPool
{
    void ReturnBulletToPool(GameObject bullet);
}

public abstract class BulletBase : MonoBehaviour
{
    public float bulletLife = 5f; // Time before bullet is destroyed    
    public float speed;
    public bool isHoming = false;
    public float homingSpeed = 2f; // Speed at which the bullet adjusts its direction
    public Transform target; // Target to home onto (e.g., the player)

    [HideInInspector]
    public IBulletPool poolOwner; // Reference to the pool managing this bullet

    private float timer = 0f;

    private void Awake()
    {
        // Automatically set the target to the player if not manually assigned
        if (target == null && LevelManager.Instance != null)
        {
            target = LevelManager.Instance.playerController.transform;
        }
    }

    // Runs every time the bullet is taken from a pool, so reset per-shot state here
    protected virtual void OnEnable()
    {
        timer = 0f;
    }

    protected virtual void FixedUpdate()
    {
        timer += Time.deltaTime;

        Move();

        if (timer >= bulletLife)
        {
            ReturnToPool();
        }

        CheckBounds();
    }

    // Subclasses can override this to replace the default movement
    protected virtual void Move()
    {
        if (isHoming && target != null)
        {
            HomeOntoTarget();
        }
        else
        {
            MoveStraight();
        }
    }

    private void MoveStraight()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    public void HomeOntoTarget()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingSpeed * Time.deltaTime);
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void CheckBounds()
    {
        // Only check bounds after the bullet has been active for a short time
        if (timer < 0.1f) return;

        if (transform.position.x < PlayAreaManager.Instance.MinX - 5 ||
            transform.position.x > PlayAreaManager.Instance.MaxX + 5 ||
            transform.position.y < PlayAreaManager.Instance.MinY - 5 ||
            transform.position.y > PlayAreaManager.Instance.MaxY + 5)
        {
            ReturnToPool();
        }
    }

    protected abstract void OnTriggerEnter2D(Collider2D other);

    protected void ReturnToPool()
    {
        timer = 0f; // Reset the timer

        // Cast to Object so a destroyed owner (e.g. a spawner removed on phase change) counts as null
        if (poolOwner is Object owner && owner != null)
        {
            poolOwner.ReturnBulletToPool(gameObject); // Return the bullet to its spawner's pool
        }
        else
        {
            // A destroyed owner is expected after a phase change; only warn if no owner was ever set
            if (poolOwner == null)
            {
                CustomDebug.LogWarning($"Pool owner not set for {gameObject.name}. Destroying the bullet.");
            }
            Destroy(gameObject);
        }
    }
}
