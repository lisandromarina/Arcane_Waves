using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int health = 100;
    public Action onDeath;
    public bool IsAlive { get; set; } = true;

    protected Character_Base characterBase;
    protected IMovement movement;

    protected virtual void Awake()
    {
        characterBase = GetComponent<Character_Base>();
        movement = GetComponent<IMovement>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Handle death (e.g., play animation, destroy game object)
        IsAlive = false;
        characterBase.PlayDeadAnim();

        Debug.Log($"{gameObject.name} is dying. IsAlive: {IsAlive}");

        if (onDeath != null)
        {
            Debug.Log($"{gameObject.name} invoking onDeath event.");
            onDeath.Invoke();
        }

        if (movement != null)
        {
            movement.StopMoving();
        }

        HandleDeathRewards();
        //Destroy(gameObject);
    }

    private void HandleDeathRewards()
    {
        // Check if this is an enemy and reward money
        if (gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.AddMoney(10);
        }
    }
}
