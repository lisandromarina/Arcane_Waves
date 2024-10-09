using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieryBeast : Enemy
{
    [SerializeField] private GameObject spellPrefab;  // The spell prefab to instantiate
    [SerializeField] private GameObject warningCirclePrefab;  // The warning circle prefab
    [SerializeField] private float castRadius = 150f;   // The radius around the FieryBeast to spawn the spells
    [SerializeField] private int numberOfSpells = 10;  // Number of spells to instantiate around the beast
    [SerializeField] private float warningDuration = 4f;  // How long to show the warning circle before the spell

    private List<Vector3> spellPositions = new List<Vector3>(); // Store spell positions
    private bool isCasting = false; // Track if casting spell
    private float castTimer = 0f; // Timer to track the warning duration
    public int specialDamage = 150;

    private List<GameObject> warningCircles = new List<GameObject>();

    public int mana = 0;            // Current mana
    public int maxMana = 5;         // Mana needed to cast a spell

    protected override void DamageTrigger()
    {
        base.DamageTrigger();
        ChargeMana();
    }

    private void ChargeMana()
    {
        mana++;

        if (mana >= maxMana)
        {
            StartSpellCast();
            mana = 0;
        }
    }

    private void StartSpellCast()
    {
        Debug.Log("FieryBeast starts casting a spell!");
        canAttack = false;
        currentState = State.Idle;
        isAttacking = false;
        characterBase.PlayAttackAnim(false);
        // Clear any previous warning circles
        foreach (GameObject warningCircle in warningCircles)
        {
            Destroy(warningCircle);
        }
        warningCircles.Clear();

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

        // Destroy all warning circles before casting spells
        foreach (GameObject warningCircle in warningCircles)
        {
            Destroy(warningCircle);
        }
        warningCircles.Clear(); // Clear the list after destruction

        // Instantiate the actual spell at each stored position (from warning circle)
        
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


    void Update()
    {
        base.Update();

        // If we are in the process of casting a spell, track the time
        if (isCasting)
        {
            canAttack = false;
            currentState = State.Idle;
            isAttacking = false;

            castTimer += Time.deltaTime;

            // Once the warning duration has passed, cast the spell
            if (castTimer >= warningDuration)
            {

                currentState = State.Spelling;
                //CastSpell();
            }
        }
    }

    public void SpecialAttackEnds()
    {
        currentState = State.Search;
        canAttack = true;
        isSpelling = false;
    }
}
