using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSkillScript : MonoBehaviour
{
    [SerializeField] private float radius = 110f; // Radius within which targets will be detected and damaged
    private SpriteRenderer damageRadiusIndicator; // Reference to the sprite renderer for the radius indicator

    private void Start()
    {
        // Try to find the SpriteRenderer component in child objects
        damageRadiusIndicator = GetComponentInChildren<SpriteRenderer>();

        if (damageRadiusIndicator == null)
        {
            Debug.LogWarning("Damage Radius Indicator SpriteRenderer not found. Make sure it's attached to a child GameObject.");
        }
    }

    // Draw a visual representation of the radius in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Color of the radius visualization
        Gizmos.DrawWireSphere(transform.position, radius); // Draw a wireframe sphere to visualize the range
    }

    // Method to detect targets within range
    public List<Collider2D> DetectTargets(List<string> targets)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        List<Collider2D> targetsDetected = new List<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            // Check if the collider's tag is in the list of target tags
            if (targets.Contains(collider.tag))
            {
                targetsDetected.Add(collider);
            }
        }

        return targetsDetected;
    }

    public IEnumerator AnimateRadius(float duration)
    {
        if (damageRadiusIndicator == null)
        {
            yield break;
        }

        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = damageRadiusIndicator.transform.localScale; // Adjust this if you want a specific scale
        damageRadiusIndicator.transform.localScale = initialScale;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            damageRadiusIndicator.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        // Ensure the final scale is set correctly
        damageRadiusIndicator.transform.localScale = targetScale;
    }

    // Method to deal damage to detected targets
    public void DealDamage(List<string> targetTags, int damageAmount)
    {
        // Detect targets within the specified radius
        List<Collider2D> detectedTargets = DetectTargets(targetTags);

        // Iterate through the detected targets
        foreach (Collider2D collider in detectedTargets)
        {
            // Access the Health component of the target
            Health targetHealth = collider.GetComponent<Health>();

            // If the target has a Health component, apply damage
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageAmount);
            }
        }
    }

    public float GetRadius()
    {
        return radius;
    }
}
