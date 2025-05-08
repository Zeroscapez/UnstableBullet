using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f; // Time before bullet is destroyed
    public float rotation = 0f;
    public float speed;

    private Vector2 spawnPoint;
    private float timer = 0f;
    public bool isPlayerBullet;
    public bool hasHitPlayer; // Track if the bullet has hit the player
    public bool hasBeenGrazed; // Track if the bullet has already been grazed

    [Header("Homing Settings")]
    public bool isHoming = false; // Enable/disable homing behavior
    public float homingSpeed = 2f; // Speed at which the bullet adjusts its direction
    public Transform target; // Target to home onto (e.g., the player)

    void Start()
    {
        spawnPoint = new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
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

        // Check if the bullet is out of bounds
        CheckBounds();
    }

    private void MoveStraight()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void HomeOntoTarget()
    {
        // Calculate the direction to the target
        Vector2 direction = (target.position - transform.position).normalized;

        // Gradually rotate the bullet toward the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingSpeed * Time.deltaTime);

        // Move the bullet forward
        transform.position += transform.right * speed * Time.deltaTime;
    }

    private void CheckBounds()
    {
        if (transform.position.x < PlayAreaManager.Instance.MinX ||
            transform.position.x > PlayAreaManager.Instance.MaxX ||
            transform.position.y < PlayAreaManager.Instance.MinY ||
            transform.position.y > PlayAreaManager.Instance.MaxY)
        {
            Destroy(gameObject); // Destroy the bullet if it leaves the play area
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet && other.gameObject.CompareTag("Enemy"))
        {
            // Handle player bullet hitting an enemy
            PlayerScoreManager.Instance.AddScore(300); // Add score for hitting an enemy
            Debug.Log("Player bullet hit an enemy. Added 300 points to score.");
            Destroy(gameObject); // Destroy the bullet
        }
        else if (!isPlayerBullet)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
            {
                // Handle actual hit on the player
                Debug.Log("Enemy bullet hit the player.");
                hasHitPlayer = true;
                PlayerScoreManager.Instance.AddScore(-100);
                Destroy(gameObject); // Destroy the bullet
            }
        }
    }
}
