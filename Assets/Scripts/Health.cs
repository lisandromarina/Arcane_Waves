using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    protected int health = 100;
    public Action onDeath;
    public bool IsAlive { get; set; } = true;

    protected Character_Base characterBase;
    protected Image healthBar;
    protected Transform healthBarTransform;

    private SpriteRenderer sprite;
    private bool canTakeDamage;
    private int flickAmount = 3;
    private float flickDuration = .1f;
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

        canTakeDamage = true;

        sprite = GetComponent<SpriteRenderer>();
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
        Debug.Log($"{gameObject.name} taking damage");
        DamagePopup.Create(gameObject.transform.position + new Vector3(0, 15), damage);


        if (health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log("Start courutine flicker");
            StartCoroutine(DamageFlicker());
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
        StartCoroutine(HealthFlicker());
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        Debug.Log($"{gameObject.name} healed by {healAmount}. Current health: {health}");

        UpdateHealthBar();
    }

    IEnumerator DamageFlicker()
    {
        canTakeDamage = false;
        for (int i = 0; i < flickAmount; i++)
        {
            sprite.color = new Color(1f,1f,1f,.5f);
            yield return new WaitForSeconds(flickDuration);
            sprite.color =  Color.white;
            yield return new WaitForSeconds(flickDuration);
            canTakeDamage = true;
        }
    }

    IEnumerator HealthFlicker()
    {
        canTakeDamage = false;
        for (int i = 0; i < flickAmount; i++)
        {
            sprite.color = Color.green;
            yield return new WaitForSeconds(flickDuration);
            sprite.color = Color.white;
            yield return new WaitForSeconds(flickDuration);
            canTakeDamage = true;
        }
    }
}
