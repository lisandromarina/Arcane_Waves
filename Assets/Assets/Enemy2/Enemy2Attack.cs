
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Attack : AttackBase
{
    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake method if needed
        base.targetTags = new List<string> { "Player", "Ally" };
        base.detectionRadius = 15f;
        base.currentDamage = 10;
    }


}