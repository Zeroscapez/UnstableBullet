using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TheSunBossState
{
    Phase1P1 = 0, Phase1P2 = 1, Phase2 = 2, Phase3 = 3
}
public class TheSun : Enemy
{
    [Header("Spawner Settings")]
    public GameObject spawnerPrefab;
    public float spawnerMoveDistance = 5f;
    public float spawnerDelay = 2f;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Boss Settings")]
    public Transform player;

    [Header("State and Timer")]
    public TheSunBossState currentState = TheSunBossState.Phase1P1; 
    public float fightTimer = 0f;
    public float phase2HP = 75f;
    public float phase3HP = 50f;

    private GameObject leftSpawner;
    private GameObject rightSpawner;

    public override void Start()
    {
        this.currentHealth = maxHealth;
       
        StartPhase1P1();
    }

    private void Update()
    {
        fightTimer = LevelManager.Instance.timer;

        // Check for phase transitions based on health
        if (currentState == TheSunBossState.Phase1P1 && currentHealth <= (maxHealth * 0.9))
        {
            StartPhase1P2();
        }
        else if (currentState == TheSunBossState.Phase2 && currentHealth <= phase3HP)
        {
            TransitionToPhase3();
        }
    }

    private void StartPhase1P1()
    {
        currentState = TheSunBossState.Phase1P1;
        Debug.Log("The Sun Phase 1");

        GameObject spawner1;
        GameObject spawner2;
        // Spawn spawners for Phase 1
        float middleY = (PlayAreaManager.Instance.MinY + PlayAreaManager.Instance.MaxY) / 2;
        Vector3 leftMiddlePosition = new Vector3(PlayAreaManager.Instance.MinX, middleY, 0);
        Vector3 rightMiddlePosition = new Vector3(PlayAreaManager.Instance.MaxX, middleY, 0);

        // Spawn spawners at the calculated positions
         spawner1 =  Instantiate(spawnerPrefab, leftMiddlePosition, Quaternion.Euler(0, 0, 0));
         spawner2 = Instantiate(spawnerPrefab, rightMiddlePosition, Quaternion.Euler(0, 0, 180));
        
        ConfigureSpawner(spawner1, SpawnerType.Straight, 4f, 2f, 20f, false);
        ConfigureSpawner(spawner2, SpawnerType.Straight, 4f,2f, 20f, false);
        Debug.Log(maxHealth * 0.9);

        


       
    }

    private void StartPhase1P2()
    {
        currentState = TheSunBossState.Phase1P2;
        if (currentHealth <= (maxHealth * 0.9))
        {
            GameObject spawner3;
            GameObject spawner4;

            spawner3 = Instantiate(spawnerPrefab, leftHand.position, Quaternion.Euler(0, 0, -90));
            spawner4 = Instantiate(spawnerPrefab, rightHand.position, Quaternion.Euler(0, 0, -90));

            ConfigureSpawner(spawner3, SpawnerType.Straight, 4f, 2f, 20f, false);
            ConfigureSpawner(spawner4, SpawnerType.Straight, 4f, 2f, 20f, false);
        }
    }

    private void ConfigureSpawner(GameObject spawner, SpawnerType spawnerType, float speed, float spawnDelay, float bulletlife, bool homingPlayer)
    {
        BulletSpawner spawnerScript = spawner.GetComponent<BulletSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.activePatterns.Clear(); // Clear any existing patterns
            spawnerScript.activePatterns.Add(spawnerType); // Set the pattern to Straight
            spawnerScript.speed = speed; // Set bullet speed to 1f
            spawnerScript.enableHoming = homingPlayer; // Enable homing towards the player
            spawnerScript.bulletLife = bulletlife; // Set bullet life


        }

        StartCoroutine(SpawnerAttackSequence(spawner, Vector3.down, spawnerDelay));
    }

    private void TransitionToPhase2()
    {
        currentState = TheSunBossState.Phase2;
        Debug.Log("The Sun Phase 2");

        
        // Add new attack patterns to the spawners
        if (leftSpawner != null)
        {
            BulletSpawner spawnerScript = leftSpawner.GetComponent<BulletSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.activePatterns.Add(SpawnerType.Fan);
            }
        }

        if (rightSpawner != null)
        {
            BulletSpawner spawnerScript = rightSpawner.GetComponent<BulletSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.activePatterns.Add(SpawnerType.Spiral);
            }
        }
    }

    private void TransitionToPhase3()
    {
        currentState = TheSunBossState.Phase3;
        Debug.Log("The Sun Phase 3");

        // Add more aggressive attack patterns or spawn additional spawners
        if (leftSpawner != null)
        {
            BulletSpawner spawnerScript = leftSpawner.GetComponent<BulletSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.activePatterns.Add(SpawnerType.Explosion);
            }
        }

        if (rightSpawner != null)
        {
            BulletSpawner spawnerScript = rightSpawner.GetComponent<BulletSpawner>();
            if (spawnerScript != null)
            {
                spawnerScript.activePatterns.Add(SpawnerType.Wave);
            }
        }
    }

    private IEnumerator SpawnerAttackSequence(GameObject spawner, Vector3 moveDirection, float spawnerDelay)
    {
        Vector3 targetPosition = spawner.transform.position + moveDirection;
        while (Vector3.Distance(spawner.transform.position, targetPosition) > 0.1f)
        {
            spawner.transform.position = Vector3.MoveTowards(spawner.transform.position, targetPosition, Time.deltaTime * spawnerMoveDistance);
            yield return null;
        }

        // Wait for the delay
        yield return new WaitForSeconds(spawnerDelay);

        // Enable the spawner's firing behavior
        BulletSpawner spawnerScript = spawner.GetComponent<BulletSpawner>();
        if (spawnerScript != null)
        {
            spawnerScript.enabled = true;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // Additional logic for taking damage (e.g., visual effects)
       
    }

    protected override void Die()
    {
        base.Die();

        // Additional logic for boss death
        Debug.Log("Boss defeated! Triggering level end...");
    }

   
}
