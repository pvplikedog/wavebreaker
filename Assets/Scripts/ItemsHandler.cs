using UnityEngine;

public class ItemsHandler : MonoBehaviour
{
    [Header("Player Weapons")]
    [SerializeField] private GameObject GunWeapon;
    [SerializeField] private GameObject HardGunWeapon;
    [SerializeField] private GameObject MeleeCircleWeapon;
    [SerializeField] private GameObject MeleeBoxWeapon;
    [SerializeField] private GameObject SkyFallWeapon;
    
    [Header("Statue Weapons")]
    [SerializeField] private GameObject StatueGun;
    [SerializeField] private GameObject MassDestruct;
    [SerializeField] private GameObject FreezingWeapon;
    [SerializeField] private GameObject AuraWeapon;
    [SerializeField] private GameObject VerticalWeapon;
    [Space]
    
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

    public void AddOrUpgradeStatueGunWeapon(UpgradeOptionSO upgradeOption)
    {
        if (StatueGun.activeSelf)
        {
            StatueGun.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            StatueGun.SetActive(true);
            inventoryManager.AddWeapon(StatueGun.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeMassDestructWeapon(UpgradeOptionSO upgradeOption)
    {
        if (MassDestruct.activeSelf)
        {
            MassDestruct.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            MassDestruct.SetActive(true);
            inventoryManager.AddWeapon(MassDestruct.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeFreezingWeapon(UpgradeOptionSO upgradeOption)
    {
        if (FreezingWeapon.activeSelf)
        {
            FreezingWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            FreezingWeapon.SetActive(true);
            inventoryManager.AddWeapon(FreezingWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeAuraWeapon(UpgradeOptionSO upgradeOption)
    {
        if (AuraWeapon.activeSelf)
        {
            AuraWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            AuraWeapon.SetActive(true);
            inventoryManager.AddWeapon(AuraWeapon.GetComponent<Weapon>());
        }
    }

    public void AddOrUpgradeVerticalWeapon(UpgradeOptionSO upgradeOption)
    {
        if (VerticalWeapon.activeSelf)
        {
            VerticalWeapon.GetComponent<Weapon>().Upgrade();
        }
        else
        {
            VerticalWeapon.SetActive(true);
            inventoryManager.AddWeapon(VerticalWeapon.GetComponent<Weapon>());
        }
    }
}