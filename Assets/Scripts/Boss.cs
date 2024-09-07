using UnityEngine;

public class Boss : EnemyRange
{
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
        base.Start(); // Call the base class Start method

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
    }

    protected override void DamageTrigger()
    {
        base.DamageTrigger(); // Call base class method for ranged attack

        // Increment the ranged attack counter
        rangedAttackCount++;

        // Check if it's time to cast the special skill
        if (rangedAttackCount >= 2 && isSpecialSkillReady)
        {
            isPreparingForSpecialSkill = true; // Set flag to start preparing for the special skill
        }
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
        characterBase.PlayAttackAnim(true);
        isCastingHealing = false;
        currentState = State.Search;
    }

    public void OnHealAnimation()
    {
        Heal(maxHealth);
    }
    private void PrepareForSpecialSkill()
    {
        if (GetPlayerTransform() == null) return;

        // Move towards the player until within damage radius
        if (!IsPlayerInSkillRange())
        {
            MoveTowards(GetPlayerTransform());
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

    private bool IsPlayerInSkillRange()
    {
        if (targetTransform == null) return false;
        return Vector3.Distance(transform.position, targetTransform.position) <= jumpSkillScript.GetRadius();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the damage radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jumpSkillScript != null ? jumpSkillScript.GetRadius() : 0f);
    }
}
