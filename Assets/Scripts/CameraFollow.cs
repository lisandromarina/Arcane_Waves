using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;     // Reference to the player’s transform
    public Vector3 offset;       // Offset from the player position
    public float smoothSpeed = 0.125f; // Speed at which the camera follows

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMainMenu>().transform;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the target position with the offset
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera towards the target position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Optional: Keep the camera facing the player
            // transform.LookAt(player);
        }
    }
}
