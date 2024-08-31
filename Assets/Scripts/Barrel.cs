using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Health
{
    [SerializeField] private int healthRestoreAmount = 20; // Amount of health restored when the barrel is destroyed

    protected override void Awake()
    {
        base.Awake(); // Initialize health or any other setup
    }

    protected override void Die()
    {
        base.Die();
        Explode();
        HealPlayer(); // Heal the player when the barrel is destroyed
    }

    private void Explode()
    {
        Debug.Log("Barrel exploded!");
        // Implement explosion logic here (e.g., apply force to nearby objects, create visual effects)
    }

    private void HealPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag
        if (playerObject != null)
        {
            BaseCharacter player = playerObject.GetComponent<BaseCharacter>(); // Get the BaseCharacter component attached to the player
            if (player != null && player.IsAlive)
            {
                player.Heal(healthRestoreAmount); // Heal the player
                Debug.Log("Player healed by: " + healthRestoreAmount);
            }
        }
        else
        {
            Debug.LogWarning("No player found with the tag 'Player'.");
        }
    }

}
