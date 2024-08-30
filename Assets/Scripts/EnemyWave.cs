using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int quantity;

    public EnemyWave(GameObject prefab, int qty)
    {
        enemyPrefab = prefab;
        quantity = qty;
    }
}