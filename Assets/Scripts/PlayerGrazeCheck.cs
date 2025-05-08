// PlayerGrazeZone.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerGrazeCheck : MonoBehaviour
{
    [Header("References")]
    public Transform playerCenter;    // assign the root Player Transform
    public float hitboxRadius = 0.3f; // match your Hurtbox collider radius

    [Header("Graze Settings")]
    public int grazeScoreValue = 10;
    public LayerMask bulletLayer;

    // Track bullets currently inside the graze trigger
    public HashSet<Bullet> bulletsInRange = new HashSet<Bullet>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Bullet Enter");

        // Check if the collider belongs to a bullet
        if (((1 << col.gameObject.layer) & bulletLayer) == 0) return;
        var b = col.GetComponent<Bullet>();

        //Add bullets to set
        if (b != null)
            bulletsInRange.Add(b);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("Bullet Left");
        if (((1 << col.gameObject.layer) & bulletLayer) == 0) return;

        var bullet = col.GetComponent<Bullet>();
        if (bullet != null && bulletsInRange.Contains(bullet))
        {
            // Check if the distance between bullet and the center of the player is greater than the hitbox radius.
            float dist = Vector2.Distance(bullet.transform.position, playerCenter.position);
            if (dist > hitboxRadius && !bullet.hasHitPlayer)
            {
                // Mark the bullet as grazed and award points
                bullet.hasBeenGrazed = true;
                OnGraze(bullet);
            }

            // Remove the bullet from the set
            bulletsInRange.Remove(bullet);
        }
    }

    private void OnGraze(Bullet bullet)
    {
        // Award points, do the cool sparkles
        PlayerScoreManager.Instance.AddDisrupt(grazeScoreValue);
        PlayerScoreManager.Instance.AddScore(5000);

        Debug.Log("Grazing points awarded for bullet: " + bullet.name);
    }
}
