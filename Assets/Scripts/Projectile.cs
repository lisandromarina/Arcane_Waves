using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private Transform target;
    private float speed;
    private int damage = 10;
    private Animator animator;
    private bool isDestroying = false;
    private SpriteRenderer spriteRenderer;
    private List<string> attackTargets;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        FlipSprite();
    }

    public void SetColliderTargets(List<string> attackTargets)
    {
        this.attackTargets = attackTargets;
    }

    public void SetTarget(Vector3 position)
    {
        this.targetPosition = position;
        FlipSprite();
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void Update()
    {
        if (isDestroying) return;

        if (target != null)
        {
            MoveTowardsTarget(target.position);
        }
        else if (targetPosition != Vector3.zero)
        {
            MoveTowardsTarget(targetPosition);
        }
    }

    private void MoveTowardsTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Flip the sprite based on the direction
        FlipSprite();

        // Check if the projectile has reached the destination
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            if (target != null) HitTarget();
            else TriggerDestroy();
        }
    }

    private void FlipSprite()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0;
        }
        else if (targetPosition != Vector3.zero)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    private void HitTarget()
    {
        if (target != null)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null && targetHealth.IsAlive)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        TriggerDestroy();
    }

    private void TriggerDestroy()
    {
        if (isDestroying) return;

        isDestroying = true;
        animator.SetTrigger("Destroy");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroying) return;

        Debug.Log("Collision with: " + collision.tag);

        if (collision.transform == target)
        {
            // Check if the target is alive before applying damage
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null && targetHealth.IsAlive)
            {
                targetHealth.TakeDamage(damage);
                TriggerDestroy();
            }
        }
        else if (attackTargets.Contains(collision.tag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null && targetHealth.IsAlive)
            {
                targetHealth.TakeDamage(damage);
                TriggerDestroy();
            }
        }
    }

    public void OnDestroyAnimationEnd()
    {
        Destroy(gameObject);
    }
}
