using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSkill : MonoBehaviour
{
    private Animator animator;  // Reference to the Animator
    private List<string> attackTargets;
    private int damage;
    private bool isMoving = false;

    [SerializeField] private ParticleSystem dust;  // Reference to the particle system for the dust effect

    private bool animationFinished = false;
    private float destroyTime = 0f;
    private float waitDuration = 1f;  // Time to wait before destroying the object (1 second)

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the animator and animation are set up correctly
        if (animator != null && !animationFinished)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Check if the current animation is the "FireSkill" and if it is finished
            if (stateInfo.IsName("FireSkill") && stateInfo.normalizedTime >= 1.0f)
            {
                animationFinished = true;  // Mark the animation as finished
                
                destroyTime = Time.time + waitDuration;  // Set the time when the object should be destroyed
            }
        }

        // If the animation is finished and the wait time has passed, destroy the object
        if (animationFinished && Time.time >= destroyTime)
        {
            Destroy(gameObject);  // Destroy the object
        }
    }

    public void StartDust()
    {
        if (!isMoving)
        {
            isMoving = true;
            if (dust != null && !dust.isPlaying)
            {
                dust.Play(); // Play dust once
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
