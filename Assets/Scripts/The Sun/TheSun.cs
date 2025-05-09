using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public BossState currentState = BossState.Phase1;
    public float fightTimer = 0f;
    public float phase2HP = 75f;
    public float phase3HP = 50f;

    private GameObject leftSpawner;
    private GameObject rightSpawner;

    private void Start()
    {
        this.currentHealth = maxHealth;
        Debug.Log(currentHealth);
        StartPhase1();
    }

    private void Update()
    {
        fightTimer = LevelManager.Instance.timer;

        // Check for phase transitions based on health
        if (currentState == BossState.Phase1 && currentHealth <= phase2HP)
        {
            TransitionToPhase2();
        }
        else if (currentState == BossState.Phase2 && currentHealth <= phase3HP)
        {
            TransitionToPhase3();
        }
    }

    private void StartPhase1()
    {
        currentState = BossState.Phase1;
        Debug.Log("The Sun Phase 1");

        // Spawn spawners for Phase 1
        leftSpawner = Instantiate(spawnerPrefab, leftHand.position, Quaternion.identity);
        rightSpawner = Instantiate(spawnerPrefab, rightHand.position, Quaternion.identity);

        // Start the spawners' movement and attack sequence
        StartCoroutine(SpawnerAttackSequence(leftSpawner, Vector3.down * spawnerMoveDistance));
        StartCoroutine(SpawnerAttackSequence(rightSpawner, Vector3.down * spawnerMoveDistance));
    }

    private void TransitionToPhase2()
    {
        currentState = BossState.Phase2;
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
        currentState = BossState.Phase3;
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

    private IEnumerator SpawnerAttackSequence(GameObject spawner, Vector3 moveDirection)
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
        Debug.Log($"The Sun took {damage} damage. Current health: {currentHealth}");
    }

    protected override void Die()
    {
        base.Die();

        // Additional logic for boss death
        Debug.Log("Boss defeated! Triggering level end...");
    }
}
