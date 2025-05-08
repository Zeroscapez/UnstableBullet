using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternController : MonoBehaviour
{
    public float speed;
    public int projectileAmount;
    public float rotationSpeed;
    public Vector2 direction;
    public bool isSpiral;
    public bool isWave;
    public bool isStraight;
    public float waveFrequency;
    public float waveAmplitude;

    private float timeElapsed;
    private Bullet bullet;


    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<Bullet>();
    }

    // Update is called once per frame
    private void Update()
    {
        timeElapsed += Time.deltaTime;

        // Apply movement based on the selected pattern
        if (isSpiral)
        {
            ApplySpiralPattern();
        }
        
        if (isWave)
        {
            ApplyWavePattern();
        }

        if(isStraight)
        {
            MoveStraight();
        }
    }

    private void MoveStraight()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void ApplySpiralPattern()
    {
        // Rotate the direction vector over time
        float angle = rotationSpeed * timeElapsed;
        Vector2 rotatedDirection = new Vector2(
            Mathf.Cos(angle) * direction.x - Mathf.Sin(angle) * direction.y,
            Mathf.Sin(angle) * direction.x + Mathf.Cos(angle) * direction.y
        );

        transform.Translate(rotatedDirection.normalized * speed * Time.deltaTime, Space.World);
    }

    private void ApplyWavePattern()
    {
        // Add wave-like movement to the bullet
        Vector2 waveOffset = new Vector2(
            Mathf.Sin(timeElapsed * waveFrequency) * waveAmplitude,
            0f
        );

        transform.Translate((direction + waveOffset).normalized * speed * Time.deltaTime, Space.World);
    }

}
