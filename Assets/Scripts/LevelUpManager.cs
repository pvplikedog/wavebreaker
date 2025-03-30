using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpManager : MonoBehaviour
{
    [Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeName;
        public TMP_Text upgradeDescription;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Serializable]
    public class UpgradeOption
    {
        public int CurrentLvl;
        public UpgradeOptionSO upgradeOptionSO;
    }
    
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();
    public List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();
    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ItemsHandler itemsHandler;
    [SerializeField] private PlayerHealth playerHealth;

    private void CheckForPossibleUpgrades()
    {
        foreach (UpgradeOption upgradeOption in upgradeOptions)
        {
            if (upgradeOption.upgradeOptionSO.UpgradeType == UpgradeOptionSO.UpgradeOption.Heal)
            {
                continue;
            }
            if (upgradeOption.upgradeOptionSO.MaxLvl == upgradeOption.CurrentLvl)
            {
                upgradeOptions.Remove(upgradeOption);
                break;
            }
        }
    }
    
    private UpgradeOption GetUpgradeOption()
    {
        if (upgradeOptions.Count == 0)
        {
            return null;
        }
        return upgradeOptions[Random.Range(0, upgradeOptions.Count)];
    }
    
    private void ApplyUpgrade(UpgradeOption upgrade)
    {
        if (upgrade.CurrentLvl < upgrade.upgradeOptionSO.MaxLvl || upgrade.upgradeOptionSO.UpgradeType == UpgradeOptionSO.UpgradeOption.Heal)
        {
            if (upgrade.upgradeOptionSO.UpgradeType != UpgradeOptionSO.UpgradeOption.Heal)
            {
                upgrade.CurrentLvl++;
            }
            ChooseUpgrade(upgrade.upgradeOptionSO.UpgradeType);
        }
        else
        {
            Debug.Log("Upgrade is already at max level.");
        }
        gameManager.EndLevelUp();
    }

    public void ConfigureUpgradeUI()
    {
        CheckForPossibleUpgrades();
        foreach (UpgradeUI upgradeUI in upgradeUIOptions)
        {
            UpgradeOption upgradeOption = GetUpgradeOption();
            if (upgradeOption.upgradeOptionSO)
            {
                upgradeUI.upgradeName.text = upgradeOption.upgradeOptionSO.Names[upgradeOption.CurrentLvl];
                upgradeUI.upgradeDescription.text = upgradeOption.upgradeOptionSO.Descriptions[upgradeOption.CurrentLvl];
                upgradeUI.upgradeIcon.sprite = upgradeOption.upgradeOptionSO.Icon;
                upgradeUI.upgradeButton.onClick.RemoveAllListeners();
                upgradeUI.upgradeButton.onClick.AddListener(() => ApplyUpgrade(upgradeOption));
            }
        }
    }
    
    private void ChooseUpgrade(UpgradeOptionSO.UpgradeOption upgradeOption)
    {
        Debug.Log(upgradeOption);
        switch (upgradeOption)
        {
            case UpgradeOptionSO.UpgradeOption.Gun:
                itemsHandler.AddOrUpgradeGunWeapon();
                break;
            case UpgradeOptionSO.UpgradeOption.HardGun:
                itemsHandler.AddOrUpgradeHardGunWeapon();
                break;
            case UpgradeOptionSO.UpgradeOption.MeleeCircle:
                itemsHandler.AddOrUpgradeMeleeCircleWeapon();
                break;
            case UpgradeOptionSO.UpgradeOption.MeleeBox:
                itemsHandler.AddOrUpgradeMeleeBoxWeapon();
                break;
            case UpgradeOptionSO.UpgradeOption.SkyFall:
                itemsHandler.AddOrUpgradeSkyFallWeapon();
                break;
            case UpgradeOptionSO.UpgradeOption.Heal:
                playerHealth.Heal(25);
                break;
        }
    }
}
