using System.Collections.Generic;

public class EnemyAttack : AttackBase
{
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method if needed
        base.detectionRadius = 5f;
        base.targetTags = new List<string> { "Player", "Ally" }; ;
        base.currentDamage = 5;
        // Additional initialization for EnemyAttack
    }
}