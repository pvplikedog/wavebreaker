using UnityEngine;

public class PassiveUpgradeManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    [HideInInspector] public int healthLevel = 0;
    [HideInInspector] public int regenerationLevel = 0;
    [HideInInspector] public int speedLevel = 0;
    [HideInInspector] public int damageLevel = 0;
    [HideInInspector] public int xpLevel = 0;
    [HideInInspector] public int damageReducerLevel = 0;
    private int maxLevel = 5;

    public void UpgradeHealth()
    {
        if (healthLevel < maxLevel)
        {
            ++healthLevel;
            playerStats.MaxHealth += 10;
        }
        else
        {
            throw new System.Exception("Max level reached");
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
            throw new System.Exception("Max level reached");
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
            throw new System.Exception("Max level reached");
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
            throw new System.Exception("Max level reached");
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
            throw new System.Exception("Max level reached");
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
            throw new System.Exception("Max level reached");
        }
    }
}
