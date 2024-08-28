using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBase : AttackBase
{
    protected override void Awake()
    {
        base.Awake();
        base.targetTags = new List<string> { "Enemy" };
        base.detectionRadius = 15f;
        base.currentDamage = 50;
    }
}
