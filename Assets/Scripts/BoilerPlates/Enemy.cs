using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 100f; // Maximum health of the enemy
    public float currentHealth; // Current health of the enemy
    public int scoreValue = 100; // Score awarded when the enemy is defeated


    

    protected virtual void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        // Reduce health
        currentHealth -= damage;

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Award score to the player
        PlayerScoreManager.Instance.AddScore(scoreValue);

       

        // Destroy the enemy
        Destroy(gameObject);
    }
}
