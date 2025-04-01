using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpManager : MonoBehaviour
{
    public List<UpgradeUI> upgradeUIOptions = new();
    public List<UpgradeOption> upgradeOptions = new();

    [SerializeField] private GameManager gameManager;
    [SerializeField] private ItemsHandler itemsHandler;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private InventoryManager inventoryManager;

    // For first usage:
    private List<UpgradeOption> firstUpgradeOptions = new();
    private bool isFirst = true;

    private void Start()
    {
        foreach (var upgradeOption in upgradeOptions)
        {
            if (IsWeaponUpgrade(upgradeOption.upgradeOptionSO))
            {
                firstUpgradeOptions.Add(upgradeOption);
            }
        }
    }

    private void CheckForPossibleUpgrades()
    {
        var possibleUpgrades = new List<UpgradeOption>();
        foreach (var upgradeOption in upgradeOptions)
            if (IsPossibleUpgrade(upgradeOption))
                possibleUpgrades.Add(upgradeOption);

        upgradeOptions.Clear();
        upgradeOptions = possibleUpgrades;
    }

    private List<UpgradeOption> GetUpgradeOptions(List<UpgradeOption> upgradeOptions_)
    {
        if (upgradeOptions_.Count == 0) return null;

        var possibleUpgrades = new List<UpgradeOption>();

        if (upgradeOptions_.Count <= 4)
        {
            // We always will choose it, but I want it to be last.
            var healUpgradeOption = upgradeOptions_[0];
            foreach (var upgradeOption in upgradeOptions_)
                if (upgradeOption.upgradeOptionSO.UpgradeType == UpgradeOptionSO.UpgradeOption.Heal)
                    healUpgradeOption = upgradeOption;
                else
                    possibleUpgrades.Add(upgradeOption);

            for (var i = 0; i < 4 - upgradeOptions_.Count + 1; i++) possibleUpgrades.Add(healUpgradeOption);
        }
        else
        {
            // Not sure about it.
            possibleUpgrades = upgradeOptions_.OrderBy(x => Random.Range(0, int.MaxValue)).Take(4).ToList();
        }

        return possibleUpgrades;
    }

    private void ApplyUpgrade(UpgradeOption upgrade)
    {
        if (upgrade.CurrentLvl < upgrade.upgradeOptionSO.MaxLvl ||
            upgrade.upgradeOptionSO.UpgradeType == UpgradeOptionSO.UpgradeOption.Heal)
        {
            if (upgrade.upgradeOptionSO.UpgradeType != UpgradeOptionSO.UpgradeOption.Heal) upgrade.CurrentLvl++;
            ChooseUpgrade(upgrade.upgradeOptionSO);
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
        var currentOption = 0;
        var upgradeOptions_ = new List<UpgradeOption>();
        if (isFirst)
        {
            isFirst = false;
            upgradeOptions_ = GetUpgradeOptions(firstUpgradeOptions);
        }
        else
        {
            upgradeOptions_ = GetUpgradeOptions(upgradeOptions);
        }
        foreach (var upgradeUI in upgradeUIOptions)
        {
            var upgradeOption = upgradeOptions_[currentOption];
            currentOption++;
            if (upgradeOption.upgradeOptionSO)
            {
                upgradeUI.upgradeName.text = upgradeOption.upgradeOptionSO.Names[upgradeOption.CurrentLvl];
                upgradeUI.upgradeDescription.text =
                    upgradeOption.upgradeOptionSO.Descriptions[upgradeOption.CurrentLvl];
                upgradeUI.upgradeIcon.sprite = upgradeOption.upgradeOptionSO.Icon;
                upgradeUI.upgradeButton.onClick.RemoveAllListeners();
                upgradeUI.upgradeButton.onClick.AddListener(() => ApplyUpgrade(upgradeOption));
            }
        }
    }

    private void ChooseUpgrade(UpgradeOptionSO upgradeOption)
    {
        switch (upgradeOption.UpgradeType)
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
            case UpgradeOptionSO.UpgradeOption.healthPassive:
                itemsHandler.AddOrUpgradeHealthLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.regenPassive:
                itemsHandler.AddOrUpgradeRegenerationLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.speedPassive:
                itemsHandler.AddOrUpgradeSpeedLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.damagePassive:
                itemsHandler.AddOrUpgradeDamageLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.xpPassive:
                itemsHandler.AddOrUpgradeXPLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.damageReducePassive:
                itemsHandler.AddOrUpgradeDamageReducerLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.StatueHealthPassive:
                itemsHandler.AddOrUpgradeStatueHealthLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.StatueRegenPassive:
                itemsHandler.AddOrUpgradeStatueRegenLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.StatueDamageReducePassive:
                itemsHandler.AddOrUpgradeStatueDamageReduceLevel(upgradeOption);
                break;
            case UpgradeOptionSO.UpgradeOption.StatueFireRatePassive:
                itemsHandler.AddOrUpgradeStatueFireRateLevel(upgradeOption);
                break;
        }
    }

    private bool IsPossibleUpgrade(UpgradeOption upgradeOption)
    {
        if (upgradeOption.CurrentLvl == upgradeOption.upgradeOptionSO.MaxLvl) return false;
        if (inventoryManager.currentPassiveIndex == 6 && IsPassiveUpgrade(upgradeOption.upgradeOptionSO) &&
            upgradeOption.CurrentLvl == 0) return false;
        if (inventoryManager.currentWeaponIndex == 6 && IsWeaponUpgrade(upgradeOption.upgradeOptionSO) &&
            upgradeOption.CurrentLvl == 0) return false;
        return true;
    }

    private bool IsWeaponUpgrade(UpgradeOptionSO upgradeOption)
    {
        switch (upgradeOption.UpgradeType)
        {
            case UpgradeOptionSO.UpgradeOption.Gun:
            case UpgradeOptionSO.UpgradeOption.HardGun:
            case UpgradeOptionSO.UpgradeOption.MeleeCircle:
            case UpgradeOptionSO.UpgradeOption.MeleeBox:
            case UpgradeOptionSO.UpgradeOption.SkyFall:
                return true;
            default:
                return false;
        }
    }

    private bool IsPassiveUpgrade(UpgradeOptionSO upgradeOption)
    {
        switch (upgradeOption.UpgradeType)
        {
            case UpgradeOptionSO.UpgradeOption.healthPassive:
            case UpgradeOptionSO.UpgradeOption.regenPassive:
            case UpgradeOptionSO.UpgradeOption.speedPassive:
            case UpgradeOptionSO.UpgradeOption.damagePassive:
            case UpgradeOptionSO.UpgradeOption.xpPassive:
            case UpgradeOptionSO.UpgradeOption.damageReducePassive:
            case UpgradeOptionSO.UpgradeOption.StatueHealthPassive:
            case UpgradeOptionSO.UpgradeOption.StatueRegenPassive:
            case UpgradeOptionSO.UpgradeOption.StatueDamageReducePassive:
            case UpgradeOptionSO.UpgradeOption.StatueFireRatePassive:
                return true;
            default:
                return false;
        }
    }

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
}