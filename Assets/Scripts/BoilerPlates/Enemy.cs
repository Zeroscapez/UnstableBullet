using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 100f; // Maximum health of the enemy
    public float currentHealth; // Current health of the enemy
    public int scoreValue = 100; // Score awarded when the enemy is defeated

    protected bool isDead = false; // Destroy is delayed to end of frame, so guard against dying twice


    

    public virtual void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        // Reduce health
        currentHealth = Mathf.Max(currentHealth - damage, 0f);

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            isDead = true;
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
