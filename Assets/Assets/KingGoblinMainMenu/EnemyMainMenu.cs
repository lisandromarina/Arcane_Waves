using System.Collections;
using UnityEngine;

public class EnemyMainMenu : MonoBehaviour
{
    [SerializeField] private int wavePosition = 0;

    private Character_Base animator;
    private bool isDead = false; // State to check if the enemy is dead

    void Start()
    {
        animator = GetComponent<Character_Base>();
    }

    // Method to handle enemy death
    public void Kill(System.Action onDeathComplete)
    {
        if (!isDead) // Only trigger death if not already dead
        {
            isDead = true; // Set state to dead
            animator.PlayDeadAnim(); // Play the death animation
            StartCoroutine(HandleDeath(onDeathComplete)); // Start the coroutine to handle death
        }
    }

    // Coroutine to handle waiting for the dead animation and removing the object
    private IEnumerator HandleDeath(System.Action onDeathComplete)
    {
        // Wait for the length of the death animation
        animator.PlayDeadAnim(); // Assume this method exists to get the animation duration
        yield return new WaitForSeconds(1f);

        // Additional wait time after the animation finishes
        yield return new WaitForSeconds(1f);

        // Call the callback to notify the player that the death sequence is complete
        onDeathComplete?.Invoke();

        // Destroy the enemy object
        Destroy(gameObject);
    }

    public int GetWavePosition()
    {
        return wavePosition;
    }
}
