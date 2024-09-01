using UnityEngine;

public abstract class IACharacter : BaseCharacter
{
    public enum State
    {
        Idle,
        Search,
        Chasing,
        Attacking,
        Dead
    }

    public State currentState;
    public float detectionRadius = 10f; // Radius to detect targets
    protected Transform targetTransform;

    protected override void Update()
    {
        base.Update(); // Ensure base functionality runs
        if (currentState != State.Dead)
        {
            if (target == null)
            {
                Search();
            }
            else
            {
                UpdateState();
            }
        }
        else
        {
            HandleDeath();
        }
    }

    protected virtual void Chase(Transform target)
    {
        // Continuously move towards the target until within attack range
        if (target != null)
        {
            MoveTowards(target);

            // Check if the target is within attack range
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange)
            {
                // Switch to attacking state if within attack range
                currentState = State.Attacking;
            }
        }
    }

    private void HandleDeath()
    {
        // Handle death logic here if needed
        return;
    }

    public virtual void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                // Logic for idle state
                characterBase.PlayMoveAnim(Vector3.zero);
                break;

            case State.Search:
                // Logic for search state
                Search();
                break;

            case State.Chasing:
                if (targetTransform != null)
                {
                    Chase(targetTransform);
                }
                break;

            case State.Attacking:
                // Implement attacking logic here
                break;

            case State.Dead:
                // Logic for dead state
                break;
        }
    }

    public virtual void Search()
    {
        // Default search behavior can be overridden by derived classes
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Draw the wireframe sphere
    }

    protected void MoveTowards(Transform target)
    {
        // Move towards the target until within attack range
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            // Move towards the target if outside attack range
            characterBase.PlayMoveAnim(direction);
            Move(direction);
        }
        else
        {
            // Switch to attacking state if within attack range
            currentState = State.Attacking;
        }
    }

    public virtual void Detect()
    {
        // Clear existing target
        targetTransform = null;

        // Find all colliders within detection radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Get the BaseCharacter component of the hit collider
            BaseCharacter detectedCharacter = hitCollider.GetComponent<BaseCharacter>();

            // Check if the character is alive and if its tag is in the attackTags list
            if (detectedCharacter != null && detectedCharacter.IsAlive && attackTags.Contains(hitCollider.tag))
            {
                targetTransform = detectedCharacter.transform;
                currentState = State.Chasing;
                break; // Exit loop if a target is found
            }
        }
    }
}
