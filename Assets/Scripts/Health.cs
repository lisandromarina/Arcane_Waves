using UnityEngine;
using System;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    protected int health = 100;
    public Action onDeath;
    public bool IsAlive { get; set; } = true;

    protected Character_Base characterBase;
    protected Image healthBar;
    protected Transform healthBarTransform;

    protected virtual void Awake()
    {
        health = maxHealth;
        characterBase = GetComponent<Character_Base>();
        healthBarTransform = transform.Find("HealthBar");
        if (healthBarTransform != null)
        {
            Transform canvasTransform = healthBarTransform.Find("Canvas");

            if (canvasTransform != null)
            {
                Transform healtImageTransform = canvasTransform.Find("Health");

                if (healtImageTransform != null)
                {
                    healthBar = healtImageTransform.GetComponent<Image>();
                }
            }

            // Initially hide the health bar
            healthBarTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Show health bar only when health is less than maxHealth and character is alive
            bool shouldShowHealthBar = (health < maxHealth && IsAlive);
            healthBarTransform.gameObject.SetActive(shouldShowHealthBar);

            if (shouldShowHealthBar)
            {
                healthBar.fillAmount = (float)health / maxHealth;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
       
        if (health <= 0)
        {
            Die();
        }

        UpdateHealthBar();
    }

    protected virtual void Die()
    {
        IsAlive = false;
        characterBase.PlayDeadAnim();

        Debug.Log($"{gameObject.name} is dying. IsAlive: {IsAlive}");

        if (onDeath != null)
        {
            Debug.Log($"{gameObject.name} invoking onDeath event.");
            onDeath.Invoke();
        }



        // Hide the health bar when dead
        healthBarTransform.gameObject.SetActive(false);

        Destroy(gameObject, 5f);
    }

    public void Heal(int healAmount)
    {
        if (!IsAlive) return; // Cannot heal a dead character

        health += healAmount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        Debug.Log($"{gameObject.name} healed by {healAmount}. Current health: {health}");

        UpdateHealthBar();
    }
}
