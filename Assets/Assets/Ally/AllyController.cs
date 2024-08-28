using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 120f;
    public float detectionRadius = 10f; // Vision range
    public float attackRange = 2f; // Distance to start attacking
    public LayerMask enemyLayer; // Define which layers are considered enemies

    private Transform targetEnemy;
    private Vector3 randomDirection;
    private float changeDirectionTime = 3f; // Adjusted direction change time
    private float timeSinceLastDirectionChange = 0f;
    private Camera mainCamera;

    private AttackBase attack;
    private Character_Base animator;
    private Health health; // Reference to the Health component

    private enum State { Idle, Chasing, Attacking, Dead }
    private State currentState = State.Idle; // Current state of the ally


    void Start()
    {
        animator = GetComponent<Character_Base>();
        mainCamera = Camera.main; // Reference to the main camera
        attack = GetComponent<AttackBase>();
        health = GetComponent<Health>(); // Initialize the Health component

        if (health != null)
        {
            health.onDeath += OnDeath; // Subscribe to the onDeath event
        }
    }

    void Update()
    {
        if (currentState == State.Dead)
        {
            // If dead, do nothing
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                // Perform random movement and search for enemies
                RandomMovementWithinViewport();
                SearchForEnemy();
                break;

            case State.Chasing:
                ChaseAndAttack();
                break;

            case State.Attacking:
                // Handle attacking logic here
                if (targetEnemy != null)
                {
                    Health enemyHealth = targetEnemy.GetComponent<Health>();
                    if (enemyHealth == null || !enemyHealth.IsAlive)
                    {
                        // If the enemy is dead, reset and search for another target
                        targetEnemy = null;
                        currentState = State.Idle;
                    }
                }

                if (!attack.IsAttacking())
                {
                    // If not attacking anymore, switch to chasing or idle
                    if (targetEnemy == null)
                    {
                        currentState = State.Idle;
                    }
                    else
                    {
                        currentState = State.Chasing;
                    }
                }
                break;
        }

        // Constrain the ally’s position within the viewport
        ConstrainPositionWithinViewport();

        // Ensure the Z position stays fixed
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void RandomMovementWithinViewport()
    {
        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange > changeDirectionTime)
        {
            // Change direction
            randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized; // Z is fixed at 0
            timeSinceLastDirectionChange = 0f;

            animator.PlayMoveAnim(randomDirection);
        }

        // Calculate the next position
        Vector3 nextPosition = transform.position + randomDirection * moveSpeed * Time.deltaTime;

        // Check if the next position is within the viewport
        if (IsWithinViewport(nextPosition))
        {
            transform.Translate(randomDirection * moveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // If out of bounds, change direction
            randomDirection = GetNewRandomDirection();
        }
    }

    void SearchForEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            // Check if the enemy has a Health component and if it is still alive
            Health enemyHealth = collider.GetComponent<Health>();
            if (enemyHealth != null && enemyHealth.IsAlive)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);

                // Find the closest enemy
                if (distanceToEnemy < closestDistance)
                {
                    closestEnemy = collider.transform;
                    closestDistance = distanceToEnemy;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            currentState = State.Chasing; // Switch to chasing state
        }
    }

    void ChaseAndAttack()
    {
        if (targetEnemy == null)
        {
            currentState = State.Idle; // No target, go back to idle
            return;
        }

        float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);

        // Ensure the target is still alive before proceeding
        Health enemyHealth = targetEnemy.GetComponent<Health>();
        if (enemyHealth == null || !enemyHealth.IsAlive)
        {
            targetEnemy = null;
            currentState = State.Idle; // Reset and search for another enemy
            return;
        }

        if (distanceToEnemy > attackRange)
        {
            // Move toward the enemy
            Vector3 directionToEnemy = (targetEnemy.position - transform.position).normalized;
            directionToEnemy.z = 0f; // Ensure Z remains fixed

            transform.Translate(directionToEnemy * moveSpeed * Time.deltaTime, Space.World);
            animator.PlayMoveAnim(directionToEnemy);
        }
        else
        {
            // Attack the enemy
            Debug.Log("Attack the enemy!");
            //attack.StartAttack(); // Implement your attack logic here

            currentState = State.Attacking; // Switch to attacking state
        }
    }

    bool IsWithinViewport(Vector3 position)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
        float buffer = 0.05f; // Adjust this value based on the ally’s size

        return viewportPoint.x >= buffer && viewportPoint.x <= 1 - buffer &&
               viewportPoint.y >= buffer && viewportPoint.y <= 1 - buffer;
    }

    void ConstrainPositionWithinViewport()
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        viewportPoint.x = Mathf.Clamp(viewportPoint.x, 0.05f, 0.95f); // Adjust based on buffer
        viewportPoint.y = Mathf.Clamp(viewportPoint.y, 0.05f, 0.95f); // Adjust based on buffer
        viewportPoint.z = mainCamera.WorldToViewportPoint(new Vector3(0, 0, 0)).z; // Fixed Z position
        transform.position = mainCamera.ViewportToWorldPoint(viewportPoint);
    }

    Vector3 GetNewRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized; // Ensure Z remains fixed
    }

    private void OnDeath()
    {
        currentState = State.Dead;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize the attack range in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
