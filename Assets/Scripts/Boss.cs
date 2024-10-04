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

        // Prioritize healing if health is below threshold
        if (!hasCastSpell && IsHealthBelowThreshold(0.45f))
        {
            Debug.Log("CastHealing outisde");
            CastHealing();
            return; // Stop further processing in this frame
        }

        base.Update(); // Process other logic only if not healing

        // If preparing for special skill, move towards the player
        if (isPreparingForSpecialSkill && !isCastingHealing)
        {
            PrepareForSpecialSkill();
            return; // Stop further processing in this frame
        }
    }

    protected override void DamageTrigger()
    {
        // Do nothing if healing is in progress
        if (isCastingHealing) return;

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
        Debug.Log("CastHealing");
        // Stop all other actions and enter the healing state
        isCastingHealing = true;
        canAttack = false;
        hasCastSpell = true; // Mark that the spell has been cast
        currentState = State.Spelling;
        isPreparingForSpecialSkill = false;
        //isSpelling = false; // Ensure casting state is reset
        //characterBase.PlayHealingAnim(true); // Trigger healing animation
    }

    public void OnHealAnimationEnds()
    {
        isCastingHealing = false;
        characterBase.PlayHealingAnim(false); // Stop healing animation
        isSpelling = false;
        canAttack = true;
        currentState = State.Search; // Return to search state after healing
    }

    public void OnHealAnimation()
    {
        Heal(maxHealth); // Heal the boss to full health
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
            currentState = State.Spelling;
            isPreparingForSpecialSkill = false;
        }
    }

    protected override void CastSpell()
    {
        if (isCastingHealing)
        {
            Debug.Log("Healing is in progress...");
            isAttacking = false;
            characterBase.PlayAttackAnim(false);
            characterBase.PlayHealingAnim(true); // Start healing animation
        }
        else
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

        isSpelling = false;
        currentState = State.Search; // Resume searching after spell
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
