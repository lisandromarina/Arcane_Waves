using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BaseCharacter : Health
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected int baseDamage = 10;
    [SerializeField] protected List<string> attackTags = new List<string> { "Player", "Ally" }; // Tags to detect and attack

    protected bool isAttacking = false;
    protected Collider2D target = null;
    private Dictionary<Collider2D, BaseCharacter> detectedTargets = new Dictionary<Collider2D, BaseCharacter>();

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
            BaseCharacter character = collider.GetComponent<BaseCharacter>();
            if (character != null && character.IsAlive && attackTags.Contains(collider.tag)) // Check if the tag is in the attack list
            {
                detectedTargets[collider] = character;
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

    public virtual void DamageTrigger()
    {
        if (target != null)
        {
            if (detectedTargets.TryGetValue(target, out BaseCharacter targetCharacter))
            {
                if (targetCharacter.IsAlive) targetCharacter.TakeDamage(CalculateDamage());
            }
        }
    }

    protected virtual int CalculateDamage()
    {
        return baseDamage;
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
        if (IsAlive)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    private bool IsTargetValid(Collider2D currentTarget)
    {
        // Check if the target is alive and within range
        BaseCharacter targetCharacter = currentTarget.GetComponent<BaseCharacter>();
        if (targetCharacter == null || !targetCharacter.IsAlive)
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
