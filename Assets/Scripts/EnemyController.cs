using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f; // Speed of enemy movement
    public float fireRate = 1.5f; // Time between shots
    public GameObject bulletPrefab; // Bullet prefab to shoot
    public Transform firePoint; // Point from which bullets are fired
    public bool moveRandom = false;
   

    private Vector2 targetPosition;

    void Start()
    {
        // Set an initial random target position
        if (moveRandom)
        {
            SetRandomTargetPosition();
        }
        else
        {
          
        }
        // Start shooting projectiles
       // StartCoroutine(ShootProjectiles());
    }

    void Update()
    {
      

        // If the enemy reaches the target position, set a new random target
        if (moveRandom && Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        float randomX = Random.Range(PlayAreaManager.Instance.MinX, PlayAreaManager.Instance.MaxX);
        float randomY = Random.Range(PlayAreaManager.Instance.MinY, PlayAreaManager.Instance.MaxY);
        targetPosition = new Vector2(randomX, randomY);
    }

    //IEnumerator ShootProjectiles()
    //{
    //    while (true)
    //    {
    //        if (bulletPrefab != null && firePoint != null)
    //        {
    //            // Instantiate the bullet
    //            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

    //            Bullet bulletScript = bullet.GetComponent<Bullet>();

               

                

    //            if (bulletScript != null)
    //            {
    //                bulletScript.isPlayerBullet = false;
    //            }
    //            else
    //            {
    //                Debug.LogError("Bullet script not found on bulletPrefab!");
    //            }
               
    //        }
    //        yield return new WaitForSeconds(fireRate); // Wait before firing the next shot
    //    }
    //}
}
