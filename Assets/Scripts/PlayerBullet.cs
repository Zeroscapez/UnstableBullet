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
            Debug.LogError("Player not found in the scene!");
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Ensure the bullet's horizontal position matches the player's position
        if (playerTransform != null)
        {
            transform.position = new Vector2(playerTransform.position.x, transform.position.y + 1f);
        }
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
