using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieryBeast : Enemy
{
    [SerializeField] private GameObject spellPrefab;  // The spell prefab to instantiate
    [SerializeField] private GameObject warningCirclePrefab;  // The warning circle prefab
    [SerializeField] private float castRadius = 150f;   // The radius around the FieryBeast to spawn the spells
    [SerializeField] private int numberOfSpells = 10;  // Number of spells to instantiate around the beast
    [SerializeField] private float warningDuration = 0f;  // How long to show the warning circle before the spell
    [SerializeField] private ParticleSystem dust;

    private List<Vector3> spellPositions = new List<Vector3>(); // Store spell positions
    private bool isCasting = false; // Track if casting spell
    private float castTimer = 0f; // Timer to track the warning duration
    public int specialDamage = 150;

    private List<GameObject> warningCircles = new List<GameObject>();

    public int mana = 0;            // Current mana
    public int maxMana = 5;         // Mana needed to cast a spell


    void Update()
    {
        base.Update();

        if (isCasting)
        {
            castTimer += Time.deltaTime;

            if (castTimer >= warningDuration)
            {
                currentState = State.Spelling;
            }
        }
    }
    protected override void DamageTrigger()
    {
        base.DamageTrigger();
        dust.Play();
        ChargeMana();
    }

    protected override void MoveTowards(Transform target)
    {
        base.MoveTowards(target);
        // Move towards the target until within attack range
        Vector3 direction = (target.position - transform.position).normalized;

        // Adjust dust particle effect position based on movement direction
        Vector3 dustPosition = dust.transform.localPosition;
        float offset = 0.1f; // Offset value to place the dust to the left or right of the beast

        if (direction.x < 0)
        {
            // Moving left, set dust position to the left of the beast
            dustPosition.x = -Mathf.Abs(offset);
        }
        else
        {
            // Moving right, set dust position to the right of the beast
            dustPosition.x = Mathf.Abs(offset);
        }

        dust.transform.localPosition = dustPosition;
    }

    private void ChargeMana()
    {
        mana++;

        if (mana >= maxMana)
        {
            ShowWarnings();
            mana = 0;
        }
    }

    private void ShowWarnings()
    {
        Debug.Log("FieryBeast starts casting a spell!");
        canAttack = false;
        isAttacking = false;
        characterBase.PlayAttackAnim(false);
        currentState = State.Idle;

        // Spawn the warning circles at random positions around the FieryBeast
        Vector3 fieryBeastPosition = transform.position;
        spellPositions.Clear();

        for (int i = 0; i < numberOfSpells; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float randomRadius = Random.Range(0f, castRadius);
            float x = Mathf.Cos(angle) * randomRadius;
            float y = Mathf.Sin(angle) * randomRadius;
            Vector3 spellPosition = new Vector3(x, y, 0) + fieryBeastPosition;

            // Instantiate the warning circle and add it to the list
            GameObject warningCircleInstance = Instantiate(warningCirclePrefab, spellPosition, Quaternion.identity);
            warningCircles.Add(warningCircleInstance);

            spellPositions.Add(spellPosition); // Store the positions for spell casting
        }

        // Start tracking time to cast the spell
        isCasting = true;
        castTimer = 0f;
    }

    protected override void CastSpell()
    {
        Debug.Log("FieryBeast casts a spell!");

        characterBase.PlaySpecialSkillAnim("specialSkill");
       
    }

    public void SpawnSpellsInRadius()
    {
        foreach (Vector3 spellPosition in spellPositions)
        {
            // Instantiate the spell first to access its sprite properties
            GameObject spellInstance = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
            spellInstance.GetComponent<FireSkill>().SetTargets(attackTags);
            spellInstance.GetComponent<FireSkill>().SetDamage(specialDamage);
            // Adjust the position of the spell based on its sprite bounds
            SpriteRenderer spriteRenderer = spellInstance.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Get the height of the sprite in world units (based on its bounds)
                float spriteHeight = spriteRenderer.bounds.size.y;

                // Adjust the y position to align the bottom of the spell to the warning position
                Vector3 adjustedPosition = new Vector3(spellPosition.x, spellPosition.y + (spriteHeight / 2), spellPosition.z);

                // Set the adjusted position to the spell
                spellInstance.transform.position = adjustedPosition;
            }
        }

        // End the spell casting process
        isCasting = false;
        currentState = State.Search;
        canAttack = true;
        isSpelling = false;

    }

    public void SpecialAttackEnds()
    {
        currentState = State.Search;
        canAttack = true;
        isSpelling = false;
    }
}
