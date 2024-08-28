using System;
using UnityEngine;

public class UpgradeSystem
{

    private int playerCoins = 500;

    public int playerUpgradeLevel = 0;
    public int tankUpgradeLevel = 0;
    public int mageUpgradeLevel = 0;
    public int warriorUpgradeLevel = 0;

    private float playerBaseCost = 100f;
    private float tankBaseCost = 150f;
    private float mageBaseCost = 200f;
    private float warriorBaseCost = 175f;

    private float playerScalingFactor = 0.2f;
    private float tankScalingFactor = 0.25f;
    private float mageScalingFactor = 0.3f;
    private float warriorScalingFactor = 0.25f;

    public float GetUpgradeCost(float baseCost, int upgradeLevel, float scalingFactor)
    {
        return baseCost * Mathf.Pow(1 + scalingFactor, upgradeLevel);
    }

    // Method to check if the player has enough coins to afford an upgrade or item
    public bool CanAfford(float cost)
    {
        return playerCoins >= cost;
    }

    // Method to deduct the coins when a purchase is made
    public void DeductCoins(float cost)
    {
        playerCoins -= (int)cost;
    }


    public void PurchasePlayerUpgrade()
    {
        float cost = GetUpgradeCost(playerBaseCost, playerUpgradeLevel, playerScalingFactor);
        if (CanAfford(cost))
        {
            playerUpgradeLevel++;
            DeductCoins(cost);
            // Apply upgrade logic
        }
    }

    public void PurchaseNewTank()
    {
        float cost = GetUpgradeCost(tankBaseCost, tankUpgradeLevel, tankScalingFactor);
        if (CanAfford(cost))
        {
            tankUpgradeLevel++;
            DeductCoins(cost);
            // Add new tank ally logic
        }
    }

    // Repeat similar methods for mage and warrior upgrades
}