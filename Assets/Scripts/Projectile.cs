using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage = 10;
    private Animator animator; // Reference to the Animator component
    private bool isDestroying = false; // To avoid multiple calls
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        FlipSprite(); // Flip sprite initially towards the target
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
        if (isDestroying) return; // Stop updating if in the process of destroying

        if (target == null)
        {
            TriggerDestroy();
            return;
        }

        // Move towards the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Flip the sprite based on the direction
        FlipSprite();

        // Check if the projectile has reached the target
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    private void FlipSprite()
    {
        // Flip the sprite horizontally based on the x direction
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // Facing right
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true; // Facing left
            }
        }
    }

    private void HitTarget()
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        TriggerDestroy(); // Play destruction animation instead of directly destroying
    }

    private void TriggerDestroy()
    {
        if (isDestroying) return; // Avoid multiple triggers

        isDestroying = true;
        animator.SetTrigger("Destroy"); // Trigger the destroy animation
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            HitTarget();
        }
    }

    // This method is called at the end of the destroy animation
    public void OnDestroyAnimationEnd()
    {
        Destroy(gameObject); // Destroy the projectile
    }
}
