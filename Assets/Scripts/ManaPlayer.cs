using UnityEngine;
using UnityEngine.UI;
public class ManaPlayer : BaseCharacter
{
    [SerializeField] private int maxMana = 100;
    [SerializeField]private int specialSkillManaCost = 20;
    private int mana;

    private Image manaBar;
    private Transform manaBarTransform;

    private Color normalColor = new Color(0.02f, 0.2f, 1.0f); // 0052FF
    private Color readyColor = Color.white;
    private bool isVisible = true;

    protected override void Awake()
    {
        base.Awake();
        mana = 0;

        manaBarTransform = transform.Find("ManaBar");
        if (manaBarTransform != null)
        {
            Transform canvasTransform = manaBarTransform.Find("Canvas");

            if (canvasTransform != null)
            {
                Transform healtImageTransform = canvasTransform.Find("Mana");

                if (healtImageTransform != null)
                {
                    manaBar = healtImageTransform.GetComponent<Image>();
                }
                else
                {
                    Debug.Log("healtImageTransform not found");
                }
            }
            else
            {
                Debug.Log("canvasTransform not found");
            }

            // Initially hide the health bar
            manaBarTransform.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Mana bar not found");
        }

    }

    public void SetIsVisible(bool isVisible)
    {
        this.isVisible = isVisible;
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            if (isVisible)
            {
                float fillAmount = (float)mana / maxMana;

                // Clamp fill amount between 0 and 1
                fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
                manaBar.fillAmount = fillAmount;

                // Show mana bar only when the player is alive
                bool shouldShowManaBar = IsAlive;
                manaBarTransform.gameObject.SetActive(shouldShowManaBar);

                // Change color based on fill amount
                if (fillAmount >= 1.0f)
                {
                    manaBar.color = readyColor; // Change color to indicate ready state
                }
                else
                {
                    manaBar.color = normalColor; // Reset to normal color
                }

                // Log the current state
                Debug.Log($"Updating mana bar: Mana = {mana}, Max Mana = {maxMana}, Fill Amount = {fillAmount}");
            }
        }
    }

    public void UseMana(int manaCost)
    {
        if (mana < manaCost)
        {
            Debug.Log("Not enough mana");
            return;
        }

        mana -= manaCost;
        Debug.Log($"{gameObject.name} used {manaCost} mana. Current mana: {mana}");

        UpdateManaBar();
    }

    public void UseMana()
    {
        if (mana < specialSkillManaCost)
        {
            Debug.Log("Not enough mana");
            return;
        }

        mana -= specialSkillManaCost;
        Debug.Log($"{gameObject.name} used {specialSkillManaCost} mana. Current mana: {mana}");

        UpdateManaBar();
    }

    public void RegenerateMana(int amount)
    {
        mana += amount;

        if (mana > maxMana)
        {
            mana = maxMana;
        }

        UpdateManaBar();
    }

    public bool HasEnoughMana()
    {
        return mana >= specialSkillManaCost;
    }

}