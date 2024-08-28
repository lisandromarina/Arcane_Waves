using UnityEngine;

public class Enemy : IACharacter
{

    private Transform playerTransform; // Reference to the player's transform

    private void Start()
    {
        // Find the player at the start of the game
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found in the scene. Make sure the player has the correct tag.");
        }
    }

    public override void Search()
    {
        Debug.Log("Searching");

        if (targetTransform != null)
        {
            // Move towards detected target
            MoveTowards(targetTransform);
        }
        else if (playerTransform != null)
        {
            // Move towards the player if no specific target is found
            MoveTowards(playerTransform);
        }
        else
        {
            // Implement alternative behavior if the player is also not found
            Debug.Log("No target detected and player position is unknown.");
        }

        // Continue detecting other targets or potentially switch to chasing
        Detect();
    }
}
