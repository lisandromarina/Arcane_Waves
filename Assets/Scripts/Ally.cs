using UnityEngine;

public class Ally : IACharacter
{
    [SerializeField] private float followRange = 15f; // Distance to maintain from the player
    [SerializeField] private float moveInterval = 1f; // Time in seconds before moving randomly
    [SerializeField] private float bufferZone = 2f; // Buffer zone distance to avoid constant toggling
    [SerializeField] private float speed = 5f; // Movement speed

    private float moveTimer = 0f; // Timer for random movement
    private Transform playerTransform;
    private Vector2 currentDirection;
    private bool isWithinBufferZone;

    private void Start()
    {
        currentState = State.Search;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        SetRandomDirection(); // Set initial random direction
    }

    public override void Search()
    {
        if (targetTransform != null)
        {
            // Move towards detected target
            MoveTowards(targetTransform);
        }
        else if (playerTransform != null)
        {
            // Calculate the distance to the player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > followRange)
            {
                // Reset the move timer when moving towards the player
                moveTimer = 0f;

                // Move closer if the ally is too far from the player
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                Move(direction);
                characterBase.PlayMoveAnim(direction);
                isWithinBufferZone = false;
            }
            else
            {
                // Check if the ally is within the buffer zone
                if (distanceToPlayer < (followRange - bufferZone))
                {
                    isWithinBufferZone = true;
                }

                // Handle random movement when within follow range but outside buffer zone
                if (isWithinBufferZone)
                {
                    moveTimer += Time.deltaTime;

                    if (moveTimer >= moveInterval)
                    {
                        SetRandomDirection();
                        moveTimer = 0f;
                    }

                    Move(currentDirection);
                    characterBase.PlayMoveAnim(currentDirection);
                }
            }
        }
        Detect();
    }

    private void SetRandomDirection()
    {
        // Set a new random direction
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
