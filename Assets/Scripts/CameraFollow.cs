using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;     // Reference to the player's transform
    public Vector3 offset;        // Offset from the player position
    public float smoothSpeed = 0.125f; // Speed at which the camera follows
    public float touchSensitivity = 0.05f; // Sensitivity of the touch input for moving the camera
    public bool hasToMove = true; // Flag to determine if the camera should follow the player or use touch input

    private Vector2 touchStart;  // To store the starting position of the touch
    private bool isTouching = false;  // Flag to check if the user is dragging
    private bool wasTouched = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (!wasTouched)
        {
            // Calculate the target position with the offset (ignoring horizontal movement)
            HandleOnFollow();
        }

        HandleTouchInput();
    }

    private void HandleOnFollow()
    {
        // Calculate the target position (only the y-axis is updated, x and z remain the same)
        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);

        // Clamp the y position of the target to stay within the top and bottom limits
        targetPosition.y = Mathf.Clamp(targetPosition.y, -6f, 176f);  // Adjust the limits as necessary

        // Smoothly move the camera towards the clamped target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            wasTouched = true;
            if (touch.phase == TouchPhase.Began)
            {
                // Store the start position when the user first touches the screen
                touchStart = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                // Calculate the difference in touch movement
                Vector2 touchDelta = touch.deltaPosition;

                // Move the camera up or down based on the touch movement in the y-axis
                float newYPosition = transform.position.y - touchDelta.y * touchSensitivity;

                // Clamp the newYPosition so it stays within the top and bottom limits
                newYPosition = Mathf.Clamp(newYPosition, -6f, 176f);  // Adjust the limits as necessary

                // Apply the clamped y position to the camera
                transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

}