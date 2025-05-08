using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f; // Time before bullet is destroyed
    public bool isPlayerBullet;
    public bool hasHitPlayer;// Track if the bullet has hit the player
    public bool hasBeenGrazed; // Track if the bullet has already been grazed

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
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
               this.gameObject.SetActive(false); // Deactivate the player hitbox
                Destroy(gameObject); // Destroy the bullet
            }
        }
    }

}
