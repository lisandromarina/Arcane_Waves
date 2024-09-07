using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BaseCharacter : Health
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected int baseDamage = 10;
    [SerializeField] protected List<string> attackTags = new List<string> { "Player", "Ally", "Barrel", "Training" };

    protected bool isAttacking = false;
    protected Collider2D target = null;
    private Dictionary<Collider2D, Health> detectedTargets = new Dictionary<Collider2D, Health>();

    protected override void Awake()
    {
        base.Awake(); // Ensure Health initialization
    }

    protected virtual void Update()
    {
        if (IsAlive)
        {
            if (target == null || !IsTargetValid(target))
            {
                UpdateTarget();
            }

            if (target != null)
            {
                StartAttack();
            }
            else
            {
                EndAttack();
            }
        }
        else
        {
            target = null;
            detectedTargets.Clear();
        }
    }

    private void UpdateTarget()
    {
        DetectTargets();
        SelectTarget();
    }

    private void DetectTargets()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);

        detectedTargets.Clear();
        foreach (var collider in targetsInRange)
        {
            Health healthComponent = collider.GetComponent<Health>();
            if (healthComponent != null && healthComponent.IsAlive && attackTags.Contains(collider.tag)) // Check if the tag is in the attack list
            {
                detectedTargets[collider] = healthComponent;
            }
        }
    }

    private void SelectTarget()
    {
        if (detectedTargets.Count > 0)
        {
            // Order by distance to the current position
            target = detectedTargets
                .OrderBy(pair => Vector2.Distance(transform.position, pair.Key.transform.position))
                .FirstOrDefault().Key;
        }
        else
        {
            target = null;
        }
    }

    protected virtual void DamageTrigger()
    {
        if (target != null)
        {
            if (detectedTargets.TryGetValue(target, out Health targetHealth))
            {
                if (targetHealth.IsAlive)
                {
                    targetHealth.TakeDamage(CalculateDamage());

                    Debug.Log("Target GameObject: " + target.gameObject.name);

                    // Get the Character_Base component from the GameObject associated with the Collider2D
                    Character_Base characterBase = target.gameObject.GetComponent<Character_Base>();

                    if (characterBase != null)
                    {
                        // Component found, play the animation
                        Debug.Log("Character_Base component found!");
                        characterBase.PlayTakeDamageAnim();
                    }
                    else
                    {
                        // Component not found
                        Debug.LogWarning("Character_Base component not found on the target GameObject.");
                    }
                }
            }
        }
    }

    protected virtual int CalculateDamage()
    {
        return baseDamage;
    }

    public void SetAttackRange(int attackRange)
    {
        this.attackRange = attackRange;
    }

    public void UpgradeDamage()
    {
        int increment = Mathf.RoundToInt(baseDamage * 1.5f);
        this.baseDamage = baseDamage + increment;
        Debug.Log("Player's damage upgraded to: " + baseDamage);
    }

    public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            PlayAttackAnimation(true);
        }
    }

    public void EndAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            target = null;
            PlayAttackAnimation(false);
        }
    }

    protected virtual void PlayAttackAnimation(bool isAttacking)
    {
        // Implement animation logic here
        characterBase.PlayAttackAnim(isAttacking);
    }

    protected void Move(Vector2 direction)
    {
        if (IsAlive && !isAttacking)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private bool IsTargetValid(Collider2D currentTarget)
    {
        // Check if the target is alive and within range
        Health targetHealth = currentTarget.GetComponent<Health>();
        if (targetHealth == null || !targetHealth.IsAlive)
        {
            return false;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        return distanceToTarget <= attackRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
