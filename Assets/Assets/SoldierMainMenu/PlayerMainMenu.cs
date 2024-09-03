using UnityEngine;

public class PlayerMainMenu : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints to move through
    public float speed = 5f;      // Speed of the player movement
    private int currentWaypointIndex = 0;
    private bool isAttacking = false;  // State to check if the player is attacking
    private bool isMoving = true;      // State to check if the player is moving

    private Character_Base animator;   // Reference to the animation handler
    public GameConfig gameConfig;       // Reference to the GameConfig ScriptableObject

    void Start()
    {
        animator = GetComponent<Character_Base>();

        // Load the saved state
        LoadPlayerState();

        animator.PlayMoveAnim(Vector3.zero); // Start default movement animation

        StartMovement(gameConfig.bestWave); // Start movement based on bestWave from GameConfig
    }

    void Update()
    {
        if (!isAttacking && isMoving) // Only move if not attacking and movement is allowed
        {
            MoveToWaypoint();
        }
    }

    public void StartMovement(int maxWave)
    {
        currentWaypointIndex = CalculateWaypointIndex(maxWave) - 1;
        Debug.Log("currentWaypointIndex:" + currentWaypointIndex);
        if (currentWaypointIndex >= 0)
        {
            isMoving = true;
            SavePlayerState(maxWave); // Save the state whenever movement starts
        }
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length || currentWaypointIndex < 0) return; // No valid waypoint to move to

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Play the move animation in the direction of the next waypoint
        animator.PlayMoveAnim((targetWaypoint.position - transform.position).normalized);

        // Check if the player has reached the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            StopMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isAttacking)
        {
            isAttacking = true; // Set attacking state to true
            animator.PlayMoveAnim(Vector3.zero);
            animator.PlayAttackAnim(isAttacking); // Play the attack animation

            // "Kill" the enemy and wait for the death animation to complete
            other.GetComponent<EnemyMainMenu>().Kill(() =>
            {
                ResumeMovementAfterAttack();
            });
        }
    }

    private void ResumeMovementAfterAttack()
    {
        isAttacking = false; // Reset attacking state
        animator.PlayAttackAnim(false); // Reset attack animation
    }

    private int CalculateWaypointIndex(int wave)
    {
        // Calculate the closest waypoint index that is less than or equal to the wave value
        int maxIndex = Mathf.FloorToInt((float)wave / 5);
        // Ensure index is within bounds
        Debug.Log("Calculated Index: " + maxIndex);
        return Mathf.Clamp(maxIndex, 0, waypoints.Length - 1); // Adjust for zero-based index
    }

    private void StopMovement()
    {
        isMoving = false; // Set moving state to false
        animator.PlayMoveAnim(Vector3.zero); // Play idle animation
    }

    private void SavePlayerState(int maxWave)
    {
        // Save the current waypoint index to the GameConfig
        gameConfig.bestWave = maxWave;
    }

    private void LoadPlayerState()
    {
        // Load the waypoint index from GameConfig
        currentWaypointIndex = CalculateWaypointIndex(gameConfig.bestWave) - 1;
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = waypoints.Length - 1; // Ensure index is within bounds
        }
        transform.position = waypoints[currentWaypointIndex].position; // Set player position to the saved waypoint

        // Remove enemies between player and waypoint
        RemoveEnemiesBelowMaxWave(gameConfig.bestWave);
    }

    private void RemoveEnemiesBelowMaxWave(int maxWave)
    {
        // Get all enemies tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyMainMenu enemyScript = enemy.GetComponent<EnemyMainMenu>();

            if (enemyScript != null)
            {
                int enemyIndexPosition = enemyScript.GetWavePosition();

                // Check if the enemy's indexPosition is less than the maxWave
                if (enemyIndexPosition <= maxWave)
                {
                    Destroy(enemy);
                }
            }
        }
    }


    private bool IsPointOnLineSegment(Vector3 start, Vector3 end, Vector3 point)
    {
        // Check if point is on the line segment between start and end
        float lineSegmentLength = Vector3.Distance(start, end);
        float distanceToStart = Vector3.Distance(start, point);
        float distanceToEnd = Vector3.Distance(end, point);

        // Allow a small tolerance for precision issues
        const float tolerance = 0.1f;

        return Mathf.Abs(distanceToStart + distanceToEnd - lineSegmentLength) < tolerance;
    }
}
