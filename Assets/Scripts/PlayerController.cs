using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    Vector2 moveInput;
    public float moveSpeed = 5f;

    [Header("Shooting")]

    [Header("Settings")]
    public Transform fireOrigin;
    public GameObject bulletPrefab;
   
    public int grazeScore = 1000; // Score for grazing an enemy bullet

    PlayerControls controls;
   
    
    public float fireRate = 0.2f; //Gap between shots firing
    public bool isFiring;
  
    private Coroutine firingCoroutine;

    // Event for player death
    public static event Action OnPlayerDied;
    private void Awake()
    {
        controls = new PlayerControls();

        // Bind the movement actions to the Move method
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Start firing when the fire button is pressed
        controls.Player.Fire.performed += ctx => StartFiring();
        // Stop firing when the fire button is released
        controls.Player.Fire.canceled += ctx => StopFiring();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + delta;

        // Clamp the position using PlayAreaManager
        newPosition.x = Mathf.Clamp(newPosition.x, PlayAreaManager.Instance.MinX, PlayAreaManager.Instance.MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, PlayAreaManager.Instance.MinY, PlayAreaManager.Instance.MaxY);

        transform.position = newPosition;
    }

    void Fire()
    {
        if (bulletPrefab != null && fireOrigin != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, fireOrigin.position, fireOrigin.rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isPlayerBullet = true; // Set the bullet as a player bullet
            }
            else
            {
                Debug.LogError("Bullet script not found on bulletPrefab!");
            }

                if (rb != null)
            {
                rb.velocity = fireOrigin.up * 10f; // Adjust speed as needed
                Debug.Log($"Bullet fired with velocity: {rb.velocity}");
            }
            else
            {
                Debug.LogError("Rigidbody2D not found on bulletPrefab!");
            }
        }
    }

    void StartFiring()
    {
        if (firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
    }

    void StopFiring()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true) // Keep firing until the coroutine is stopped
        {
            Fire();
            yield return new WaitForSeconds(fireRate); // Wait for the next shot
        }
    }

    private void OnDestroy()
    {
        // Notify subscribers that the player has died
        OnPlayerDied?.Invoke();
    }

   
}

