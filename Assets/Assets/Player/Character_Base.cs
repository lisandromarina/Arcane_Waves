using UnityEngine;

public class Character_Base : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    public void PlayMoveAnim(Vector3 moveDir)
    {
        // Calculate speed based on movement direction
        float speed = moveDir.magnitude;

        // Set animator speed parameter
        animator.SetFloat("speed", speed);

        // Flip the sprite based on direction (left or right)
        if (moveDir.x != 0)
        {
            spriteRenderer.flipX = moveDir.x < 0; // Flip sprite when moving left
        }
    }

    public void PlayAttackAnim(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    public void PlayDeadAnim()
    {
        animator.SetBool("isDead", true);
    }

    public void PlayReviveAnim()
    {
        animator.SetBool("isDead", false);
    }

    public void PlayHealingAnim(bool isHealing)
    {
        animator.SetBool("isHealing", isHealing);
    }

    public void PlaySpecialSkillAnim(string skill)
    {
        animator.SetTrigger(skill);
    }

    public void PlayTakeDamageAnim()
    {
        animator.SetTrigger("TakeDamage");
    }
}
