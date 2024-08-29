using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Base : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayMoveAnim(Vector3 moveDir)
    {
        // Calculate speed based on movement direction
        float speed = moveDir.magnitude;

        // Set animator speed parameter
        animator.SetFloat("speed", speed);

        // Flip sprite based on direction (left or right)
        if (moveDir.x != 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Sign(moveDir.x) * Mathf.Abs(newScale.x);
            transform.localScale = newScale;
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

}
