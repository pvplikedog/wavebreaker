using UnityEngine;

public class ItemsHandler : MonoBehaviour
{
    [SerializeField] private GameObject GunWeapon;
    [SerializeField] private GameObject HardGunWeapon;
    [SerializeField] private GameObject MeleeCircleWeapon;
    [SerializeField] private GameObject MeleeBoxWeapon;
    [SerializeField] private GameObject SkyFallWeapon;
    
    [SerializeField] private InventoryManager inventoryManager;
    
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
}
