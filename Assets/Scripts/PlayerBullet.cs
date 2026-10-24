using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BulletBase
{
    private Transform playerTransform; // Reference to the player's transform

    private void Start()
    {
        // Find the player and store its transform
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            CustomDebug.LogError("Player not found in the scene!");
        }
    }

    protected override void Move()
    {
        // Travel straight up at 'speed' while keeping the bullet lined up with the player's x position
        float x = playerTransform != null ? playerTransform.position.x : transform.position.x;
        transform.position = new Vector2(x, transform.position.y + speed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method to take damage
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(90f); // Adjust damage as needed
                PlayerScoreManager.Instance.AddScore(100);
            }
            // Destroy the bullet upon hitting an enemy
            ReturnToPool();
        }
    }
}
