using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method to take damage
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1f); // Adjust damage as needed
            }
            // Destroy the bullet upon hitting an enemy
            Destroy(gameObject);
        }
       
    }
}
