using UnityEngine;

public class ItemsHandler : MonoBehaviour
{
    [SerializeField] private GameObject GunWeapon;
    [SerializeField] private GameObject HardGunWeapon;
    [SerializeField] private GameObject MeleeCircleWeapon;
    [SerializeField] private GameObject MeleeBoxWeapon;
    [SerializeField] private GameObject SkyFallWeapon;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PassiveUpgradeManager passiveUpgradeManager;

    public void AddOrUpgradeGunWeapon()
    {
        if (GunWeapon.activeSelf)
        {
            GunWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            GunWeapon.SetActive(true);
            inventoryManager.AddWeapon(GunWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeHardGunWeapon()
    {
        if (HardGunWeapon.activeSelf)
        {
            HardGunWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            HardGunWeapon.SetActive(true);
            inventoryManager.AddWeapon(HardGunWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeMeleeCircleWeapon()
    {
        if (MeleeCircleWeapon.activeSelf)
        {
            MeleeCircleWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            MeleeCircleWeapon.SetActive(true);
            inventoryManager.AddWeapon(MeleeCircleWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeMeleeBoxWeapon()
    {
        if (MeleeBoxWeapon.activeSelf)
        {
            MeleeBoxWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            MeleeBoxWeapon.SetActive(true);
            inventoryManager.AddWeapon(MeleeBoxWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeSkyFallWeapon()
    {
        if (SkyFallWeapon.activeSelf)
        {
            SkyFallWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            SkyFallWeapon.SetActive(true);
            inventoryManager.AddWeapon(SkyFallWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeHealthLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.healthLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeHealth();
    }

    public void AddOrUpgradeRegenerationLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.regenerationLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeRegeneration();
    }

    public void AddOrUpgradeSpeedLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.speedLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeSpeed();
    }

    public void AddOrUpgradeDamageLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.damageLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeDamage();
    }

    public void AddOrUpgradeXPLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.xpLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeXP();
    }

    public void AddOrUpgradeDamageReducerLevel(UpgradeOptionSO upgradeOptionSO)
    {
        if (passiveUpgradeManager.damageReducerLevel == 0) inventoryManager.AddPassive(upgradeOptionSO);

        passiveUpgradeManager.UpgradeDamageReducer();
    }

    public void AddOrUpgradeStatueHealthLevel(UpgradeOptionSO upgradeOption)
    {
        if (passiveUpgradeManager.statueHealthLevel == 0) inventoryManager.AddPassive(upgradeOption);

        passiveUpgradeManager.UpgradeStatueHealth();
    }

    public void AddOrUpgradeStatueRegenLevel(UpgradeOptionSO upgradeOption)
    {
        if (passiveUpgradeManager.statueRegenLevel == 0) inventoryManager.AddPassive(upgradeOption);

        passiveUpgradeManager.UpgradeStatueRegen();
    }

    public void AddOrUpgradeStatueDamageReduceLevel(UpgradeOptionSO upgradeOption)
    {
        if (passiveUpgradeManager.statueDamageReducerLevel == 0) inventoryManager.AddPassive(upgradeOption);

        passiveUpgradeManager.UpgradeStatueDamageReduce();
    }

    public void AddOrUpgradeStatueFireRateLevel(UpgradeOptionSO upgradeOption)
    {
        if (passiveUpgradeManager.statueFireRateLevel == 0) inventoryManager.AddPassive(upgradeOption);

        passiveUpgradeManager.UpgradeStatueFireRate();
    }
}