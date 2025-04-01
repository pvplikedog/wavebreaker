using System;
using UnityEngine;

public class PassiveUpgradeManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [HideInInspector] public int healthLevel;
    [HideInInspector] public int regenerationLevel;
    [HideInInspector] public int speedLevel;
    [HideInInspector] public int damageLevel;
    [HideInInspector] public int xpLevel;
    [HideInInspector] public int damageReducerLevel;

    // Statue:
    [HideInInspector] public int statueHealthLevel;
    [HideInInspector] public int statueRegenLevel;
    [HideInInspector] public int statueFireRateLevel;
    [HideInInspector] public int statueDamageReducerLevel;

    private readonly int maxLevel = 5;

    public void UpgradeHealth()
    {
        if (healthLevel < maxLevel)
        {
            ++healthLevel;
            playerStats.MaxHealth += 10;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeRegeneration()
    {
        if (regenerationLevel < maxLevel)
        {
            ++regenerationLevel;
            playerStats.RegenerationRate += 0.2f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeSpeed()
    {
        if (speedLevel < maxLevel)
        {
            ++speedLevel;
            playerStats.SpeedMultiplier += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeDamage()
    {
        if (damageLevel < maxLevel)
        {
            ++damageLevel;
            playerStats.DamageMultiplier += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeXP()
    {
        if (xpLevel < maxLevel)
        {
            ++xpLevel;
            playerStats.XPMultiplier += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeDamageReducer()
    {
        if (damageReducerLevel < maxLevel)
        {
            ++damageReducerLevel;
            playerStats.DamageReducer += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeStatueHealth()
    {
        if (statueHealthLevel < maxLevel)
        {
            ++statueHealthLevel;
            playerStats.StatueMaxHealth += 10;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeStatueRegen()
    {
        if (statueRegenLevel < maxLevel)
        {
            ++statueRegenLevel;
            playerStats.StatueRegenerationRate += 0.2f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeStatueDamageReduce()
    {
        if (statueDamageReducerLevel < maxLevel)
        {
            ++statueDamageReducerLevel;
            playerStats.StatueDamageReducer += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }

    public void UpgradeStatueFireRate()
    {
        if (statueFireRateLevel < maxLevel)
        {
            ++statueFireRateLevel;
            playerStats.StatueFireRateMultiplier += 0.1f;
        }
        else
        {
            throw new Exception("Max level reached");
        }
    }
}