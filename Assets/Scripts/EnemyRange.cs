using UnityEngine;

public class EnemyRange : IACharacter
{
    private Transform playerTransform;
    [SerializeField] private int moneyReward;
    [SerializeField] private GameObject projectilePrefab; // Reference to projectile prefab
    [SerializeField] private float projectileSpeed = 100f;
    [SerializeField] private float rangedAttackCooldown = 2f; // Time between ranged attacks
    [SerializeField] private float specialSkillCooldown = 5f; // Cooldown for the special skill
    [SerializeField] private SpriteRenderer damageRadiusIndicator; // Reference to the sprite renderer for damage radius indicator
    [SerializeField] private int damageAmount = 100;

    private JumpSkillScript jumpSkillScript; // Reference to the JumpSkillScript component

    private bool hasCastSpell = false; // Flag to check if the spell has been cast
    private bool isCastingHealing = false; // Flag to check if healing animation is playing
    private int rangedAttackCount = 0; // Counter for ranged attacks
    private bool isSpecialSkillReady = true; // Flag to indicate if the special skill is ready
    private bool isPreparingForSpecialSkill = false; // Flag to indicate if the boss is preparing to execute the special skill

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found in the scene. Make sure the player has the correct tag.");
        }

        // Ensure the indicator is hidden at the start
        if (damageRadiusIndicator != null)
        {
            damageRadiusIndicator.enabled = false;
        }

        // Find the JumpSkillScript component (Assuming it's on the same GameObject)
        jumpSkillScript = GetComponentInChildren<JumpSkillScript>();
        if (jumpSkillScript == null)
        {
            Debug.LogWarning("JumpSkillScript component not found on this GameObject.");
        }
    }

    protected override void Update()
    {
        base.Update();

        // Skip other behaviors if healing is in progress
        if (isCastingHealing)
        {
            return;
        }

        // If preparing for special skill, move towards the player
        if (isPreparingForSpecialSkill)
        {
            PrepareForSpecialSkill();
            return; // Stop further processing in this frame
        }

        // Heal if health is below threshold
        if (!hasCastSpell && IsHealthBelowThreshold(0.45f))
        {
            CastHealing();
            return; // Stop further processing in this frame
        }

        // Check if the target is still within range
        if (targetTransform != null)
        {
            if (IsTargetInRange(targetTransform))
            {
                // Do nothing if in range
            }
            else
            {
                // Chase the target if it moves out of range
                MoveTowards(targetTransform);
            }
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

    private bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    private bool IsPlayerInSkillRange()
    {
        if (target.transform == null) return false;
        return Vector3.Distance(transform.position, target.transform.position) <= jumpSkillScript.GetRadius(); // Use the radius from JumpSkillScript
    }

    private bool IsHealthBelowThreshold(float threshold)
    {
        return (float)health / maxHealth <= threshold;
    }

    private void CastHealing()
    {
        Debug.Log("Casting healing");
        isCastingHealing = true;
        hasCastSpell = true;
        characterBase.PlayAttackAnim(false);
        currentState = State.Search;
        characterBase.PlayHealingAnim(true);
    }

    public void OnHealAnimationEnds()
    {
        characterBase.PlayHealingAnim(false);
        isCastingHealing = false;
    }

    public void OnHealAnimation()
    {
        Heal(maxHealth);
    }

    protected override void Die()
    {
        base.Die();
        HandleDeathRewards();
    }

    private void HandleDeathRewards()
    {
        GameManager.Instance.AddMoney(moneyReward);
    }

    private void RangedAttack()
    {
        if (targetTransform == null || !IsTargetAlive(targetTransform)) return;

        // Store the target's position at the moment of attack
        Vector3 targetPositionAtShot = targetTransform.position;

        // Calculate the direction from the shooter to the target
        Vector3 directionToTarget = (targetPositionAtShot - transform.position).normalized;

        // Define a maximum distance for the projectile to travel
        float maxProjectileDistance = 1000f; // Set this to the desired maximum range

        // Calculate the extended target position further along the direction
        Vector3 extendedTargetPosition = transform.position + directionToTarget * maxProjectileDistance;

        // Instantiate a projectile towards the extended target position
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            // Set the projectile's extended target position and speed
            projectileComponent.SetTarget(extendedTargetPosition);
            projectileComponent.SetSpeed(projectileSpeed);
            projectileComponent.SetDamage(CalculateDamage());
        }
        else
        {
            Debug.LogWarning("Projectile component is missing on the instantiated object.");
        }

        Debug.Log("Attacking range");

        // Increment the ranged attack counter
        rangedAttackCount++;

        // Check if it's time to cast the special skill
        if (rangedAttackCount >= 2 && isSpecialSkillReady)
        {
            isPreparingForSpecialSkill = true; // Set flag to start preparing for the special skill
        }
    }

    private bool IsTargetAlive(Transform target)
    {
        Health targetHealth = target.GetComponent<Health>();
        return targetHealth != null && targetHealth.IsAlive;
    }

    private void PrepareForSpecialSkill()
    {
        if (playerTransform == null) return;

        // Move towards the player until within damage radius
        if (!IsPlayerInSkillRange())
        {
            MoveTowards(playerTransform);
        }
        else
        {
            // Once in range, perform the special skill
            CastSpecialSkill();
            isPreparingForSpecialSkill = false;
        }
    }

    private void CastSpecialSkill()
    {
        Debug.Log("Casting special skill: Jump Attack!");

        // Enable the damage radius indicator
        if (damageRadiusIndicator != null)
        {
            damageRadiusIndicator.enabled = true;
            StartCoroutine(jumpSkillScript.AnimateRadius(2f)); // Adjust the duration as needed
        }

        // Trigger the jump animation
        characterBase.PlaySpecialSkillAnim("Jump");

        // Reset the ranged attack counter
        rangedAttackCount = 0;

        // Set the special skill cooldown
        isSpecialSkillReady = false;
        Invoke(nameof(ResetSpecialSkillCooldown), specialSkillCooldown);
    }

    private void ResetSpecialSkillCooldown()
    {
        isSpecialSkillReady = true;
    }

    // This method will be called via an animation event when the boss touches the ground
    public void OnSpecialSkillLanding()
    {
        Debug.Log("Special skill landed, dealing damage!");

        // Use JumpSkillScript to deal damage
        if (jumpSkillScript != null)
        {
            jumpSkillScript.DealDamage(attackTags, damageAmount); // Use the method from JumpSkillScript to handle damage
        }
        else
        {
            Debug.LogWarning("JumpSkillScript component is not available.");
        }

        // Disable the damage radius indicator
        if (damageRadiusIndicator != null)
        {
            damageRadiusIndicator.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the damage radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jumpSkillScript != null ? jumpSkillScript.GetRadius() : 0f);
    }
}
