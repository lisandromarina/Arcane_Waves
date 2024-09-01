using UnityEngine;

public class EnemyRange : IACharacter
{
    private Transform playerTransform;
    [SerializeField] private int moneyReward;
    [SerializeField] private GameObject projectilePrefab; // Reference to projectile prefab
    [SerializeField] private float projectileSpeed = 100f;
    [SerializeField] private float rangedAttackCooldown = 2f; // Time between ranged attacks
    private bool hasCastSpell = false; // Flag to check if the spell has been cast
    private bool isCastingHealing = false; // Flag to check if healing animation is playing

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
    }

    protected override void Update()
    {
        base.Update();

        // Skip other behaviors if healing is in progress
        if (isCastingHealing)
        {
            return;
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
        if (targetTransform == null) return;

        // Instantiate a projectile towards the target
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            projectileComponent.SetTarget(targetTransform);
            projectileComponent.SetSpeed(projectileSpeed);
            projectileComponent.SetDamage(CalculateDamage());
        }
        else
        {
            Debug.LogWarning("Projectile component is missing on the instantiated object.");
        }

        Debug.Log("Attacking range");
    }
}
