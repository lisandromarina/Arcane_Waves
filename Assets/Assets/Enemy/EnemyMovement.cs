using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public LayerMask targetLayers;

    private Transform playerTransform;
    private Transform currentTarget;
    private AttackBase attack;
    private Character_Base animator;
    Health health;

    private enum State { Idle, Chasing, Attacking, Dead }
    private State currentState = State.Idle;

    void Start()
    {
        animator = GetComponent<Character_Base>();
        attack = GetComponent<AttackBase>();
        health = GetComponent<Health>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (attack.IsAttacking()) return;


        if (!health.IsAlive)
        {
            currentState = State.Dead;
            Debug.Log("dead");
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                SearchForTarget();
                MoveTowardPlayer();
                break;

            case State.Chasing:
                if (currentTarget != null && IsTargetAlive(currentTarget))
                {
                    ChaseTarget();
                }
                else
                {
                    currentTarget = null;
                    currentState = State.Idle;
                }
                break;

            case State.Attacking:
                if (currentTarget != null && IsTargetAlive(currentTarget) && Vector3.Distance(transform.position, currentTarget.position) <= attackRange)
                {
                    attack.StartAttack();
                }
                else
                {
                    currentState = State.Chasing;
                }
                break;
            case State.Dead:
                Debug.Log("dead1");
                return;
        }
    }

    void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        animator.PlayMoveAnim(directionToPlayer);
    }

    void SearchForTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayers);
        foreach (var hitCollider in hitColliders)
        {
            Transform potentialTarget = hitCollider.transform;
            Health targetHealth = potentialTarget.GetComponent<Health>();

            if (targetHealth != null && targetHealth.IsAlive)
            {
                currentTarget = potentialTarget;
                currentState = State.Chasing;
                break; // Found a valid target, exit the loop
            }
        }
    }

    void ChaseTarget()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget > attackRange)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
            animator.PlayMoveAnim(directionToTarget);
        }
        else
        {
            currentState = State.Attacking;
        }
    }

    private bool IsTargetAlive(Transform target)
    {
        Health targetHealth = target.GetComponent<Health>();
        return targetHealth != null && targetHealth.IsAlive;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
