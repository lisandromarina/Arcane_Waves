using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningCircle : MonoBehaviour
{
    public float duration = 4f; // Duration for the scale to reach full size
    private float scaleTime; // Time elapsed since the instance was created
    private Vector3 targetScale; // Final scale of the circle

    private void Start()
    {
        scaleTime = 0f; // Reset scale time
        targetScale = transform.localScale; // Store the target scale (full size)
        transform.localScale = Vector3.zero; // Start from zero scale
    }

    private void Update()
    {
        scaleTime += Time.deltaTime;

        // Calculate the interpolation factor
        float t = scaleTime / duration;
        float newScale = Mathf.Lerp(0f, targetScale.x, t); // Assuming uniform scaling

        // Update the scale of the warning circle
        transform.localScale = new Vector3(newScale, newScale, 0);

        // Destroy the instance when it has scaled up to its target size
        if (t >= 1f)
        {
            // Optionally, keep the circle around for some time or destroy it
            Destroy(gameObject); // Uncomment if you want to destroy after growing
        }
    }
}