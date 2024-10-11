using Unity.VisualScripting;
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

    public void PlayMoveAnimWithDust(Vector3 moveDir, ParticleSystem dust)
    {
        // Calculate speed based on movement direction
        float speed = moveDir.magnitude;

        // Set animator speed parameter
        animator.SetFloat("speed", speed);

        // Flip the sprite based on direction (left or right)
        if (moveDir.x != 0)
        {
            bool flip = moveDir.x < 0;
            // Get the current local scale
            Vector3 dustScale = dust.transform.localScale;
            // Modify the x value of the scale based on the flip condition
            dustScale.x = !flip ? -Mathf.Abs(dustScale.x) : Mathf.Abs(dustScale.x);
            dust.transform.localScale = dustScale;

            // Get the current local position
            Vector3 dustPosition = dust.transform.localPosition;
            // Modify the x value of the position based on the flip condition
            dustPosition.x = !flip ? -Mathf.Abs(dustPosition.x) : Mathf.Abs(dustPosition.x);
            dust.transform.localPosition = dustPosition;
            dust.Play();
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
