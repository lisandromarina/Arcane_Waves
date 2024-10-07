using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FireSkill : MonoBehaviour
{
    private Animator animator;  // Reference to the Animator
    private List<string> attackTargets;
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the animator and animation are set up correctly
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Check if the current animation is the "FireSkill" and if it is finished
            if (stateInfo.IsName("FireSkill") && stateInfo.normalizedTime >= 1.0f)
            {
                Destroy(gameObject);  // Destroy the spell if the animation is done
            }
        }
    }

    public void SetTargets(List<string> targets)
    {
        this.attackTargets = targets;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with: " + collision.tag);

        if (attackTargets.Contains(collision.tag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null && targetHealth.IsAlive)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }
}
