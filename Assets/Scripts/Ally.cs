using UnityEngine;

public class Ally : IACharacter
{
    [SerializeField] private float followRange = 15f; // Distance to maintain from the player
    [SerializeField] private float moveInterval = 1f; // Time in seconds before moving randomly
    [SerializeField] private float bufferZone = 2f; // Buffer zone distance to avoid constant toggling

    private float moveTimer = 0f; // Timer for random movement
    private Transform playerTransform;
    private Vector2 currentDirection;
    private bool isWithinBufferZone;

    // Screen boundaries
    private float minYPosition = -402f;
    private float maxYPosition = 150f;
    private float minXPosition = -349f;
    private float maxXPosition = 315f;

    private void Start()
    {
        currentState = State.Search;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        SetRandomDirection(); // Set initial random direction

        // Calculate screen boundaries
        Camera cam = Camera.main;
        minXPosition = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        maxXPosition = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
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

        // Clamp the ally's position within screen boundaries
        ClampPositionWithinScreenBounds();

        Detect();
    }

    private void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        ClampPositionWithinScreenBounds();
    }

    private void ClampPositionWithinScreenBounds()
    {
        // Clamp Y position
        float clampedY = Mathf.Clamp(transform.position.y, minYPosition, maxYPosition);
        // Clamp X position to screen boundaries
        float clampedX = Mathf.Clamp(transform.position.x, minXPosition, maxXPosition);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void SetRandomDirection()
    {
        // Set a new random direction
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
