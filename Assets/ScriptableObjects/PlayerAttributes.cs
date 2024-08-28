using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerAttributes", menuName = "Player/Attributes")]
public class PlayerAttributes : ScriptableObject
{
    public float speed;
    public float attackRange;
    public int baseDamage;
}
