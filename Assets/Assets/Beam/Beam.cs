using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private Animator animator;
    private int damageAmount = 500;
    private BeamGuy beamGuy;  // Reference to BeamGuy

    [SerializeField] private RuntimeAnimatorController leftBeamAnimatorController;   // Animator controller for left beam
    [SerializeField] private RuntimeAnimatorController rightBeamAnimatorController;  // Animator controller for right beam

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the same GameObject
        SetAnimatorController(); // Set the animator controller based on BeamGuy's position
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current animation is not playing anymore
        if (IsIdleAnimationFinished())
        {
            DestroyBeamAndBeamGuy(); // Destroy the beam and the BeamGuy when the idle animation finishes
        }
    }

    public void SetDamage(int damage)
    {
        this.damageAmount = damage;
    }

    // Set the BeamGuy reference
    public void SetBeamGuy(BeamGuy beamGuy)
    {
        this.beamGuy = beamGuy;
        SetAnimatorController(); // Set animator controller based on BeamGuy's position
    }

    // Method to set the Animator Controller based on the BeamGuy's position
    private void SetAnimatorController()
    {
        if (animator != null && beamGuy != null)
        {
            animator.runtimeAnimatorController = beamGuy.isLeftRespawn ? leftBeamAnimatorController : rightBeamAnimatorController;
        }
    }

    bool IsIdleAnimationFinished()
    {
        // Get the current state information from the Animator
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the current state is "Idle" and if the animation has finished
        return (stateInfo.IsName("Idle") || stateInfo.IsName("BeamPurple")) && stateInfo.normalizedTime >= 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Get the Enemy script component (make sure your enemy has a script with a TakeDamage method)
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                // Apply damage to the enemy
                enemy.TakeDamage(damageAmount);
            }
        }
    }

    // This function destroys both the beam and the BeamGuy
    private void DestroyBeamAndBeamGuy()
    {
        Destroy(gameObject);  // Destroy the beam

        // If BeamGuy exists, destroy it
        if (beamGuy != null)
        {
            beamGuy.DestroyBeamGuy();
        }
    }
}
