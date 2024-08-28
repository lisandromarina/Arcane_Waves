using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] protected List<string> targetTags;
    [SerializeField] protected int currentDamage = 10;

    private Collider2D target = null;
    private Character_Base characterBase;
    private bool isAttacking = false;
    private Dictionary<Collider2D, Health> detectedTargets = new Dictionary<Collider2D, Health>();

    protected virtual void Awake()
    {
        characterBase = GetComponent<Character_Base>();
    }

    protected void Update()
    {
        UpdateTarget();

        if (target != null)
        {
            StartAttack();
        }
        else
        {
            EndAttack();
        }
    }

    private void UpdateTarget()
    {
        DetectTargets();
        SelectTarget();
    }

    private void DetectTargets()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayers);

        detectedTargets.Clear();
        foreach (var collider in targetsInRange)
        {
            if (targetTags.Contains(collider.tag))
            {
                Health healthComponent = collider.GetComponent<Health>();
                if (healthComponent != null && healthComponent.IsAlive)
                {
                    detectedTargets[collider] = healthComponent;
                }
            }
        }
    }

    private void SelectTarget()
    {
        if (detectedTargets.Count > 0)
        {
            target = detectedTargets
                .OrderBy(pair => Vector2.Distance(transform.position, pair.Key.transform.position))
                .FirstOrDefault().Key;
        }
        else
        {
            target = null;
        }
    }

    public void DamageTrigger()
    {
        if (target != null)
        {
            if (detectedTargets.TryGetValue(target, out Health targetHealth) && targetHealth.IsAlive)
            {
                targetHealth.TakeDamage(CalculateDamage());
            }
        }
    }

    protected virtual int CalculateDamage()
    {
        return currentDamage;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            characterBase.PlayAttackAnim(true);
        }
    }

    public void EndAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            target = null;
            characterBase.PlayAttackAnim(false);
        }
    }

    public void SetDamage(int damage)
    {
        currentDamage += damage;
    }

    public int GetDamage()
    {
        return currentDamage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}