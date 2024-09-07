using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smooth speed for camera movement
    public Vector3 offset; // Offset from the player

    public float minX, maxX; // Minimum and maximum X boundaries
    public float minY, maxY; // Minimum and maximum Y boundaries

    void LateUpdate()
    {
        // Calculate the target position based on the player's position and the offset
        float targetX = player.position.x + offset.x;
        float targetY = player.position.y + offset.y;

        // Clamp the target position within the defined boundaries
        float clampedX = Mathf.Clamp(targetX, minX, maxX);
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

        // Create a new Vector3 for the camera's position while keeping the Z position constant
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z);

        // Smoothly interpolate from the current position to the target position
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }
}
