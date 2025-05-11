using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    public float bulletLife = 5f; // Time before bullet is destroyed    
    public float speed;
    public bool isHoming = false;
    public float homingSpeed = 2f; // Speed at which the bullet adjusts its direction
    public Transform target; // Target to home onto (e.g., the player)

    [HideInInspector]
    public BulletSpawner poolOwner; // Reference to the spawner managing this bullet

    private float timer = 0f;

    private void Awake()
    {
        // Automatically set the target to the player if not manually assigned
        if (target == null && LevelManager.Instance != null)
        {
            target = LevelManager.Instance.playerController.transform;
        }
    }

    protected virtual void FixedUpdate()
    {
        timer += Time.deltaTime;

       
            MoveStraight();
        

        if (timer >= bulletLife)
        {
            ReturnToPool();
        }

        CheckBounds();
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
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
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

        if (poolOwner != null)
        {
            poolOwner.ReturnBulletToPool(gameObject); // Return the bullet to its spawner's pool
        }
        else
        {
            Debug.LogWarning($"Pool owner not set for {gameObject.name}. Destroying the bullet.");
            Destroy(gameObject);
        }
    }
}
