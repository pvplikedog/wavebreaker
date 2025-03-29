using UnityEngine;

public class ItemsHandler : MonoBehaviour
{
    [SerializeField] private GameObject GunWeapon;
    [SerializeField] private GameObject HardGunWeapon;
    [SerializeField] private GameObject MeleeCircleWeapon;
    [SerializeField] private GameObject MeleeBoxWeapon;
    [SerializeField] private GameObject SkyFallWeapon;
    
    [SerializeField] private InventoryManager inventoryManager;
    
    public void AddGunWeapon()
    {
        GunWeapon.SetActive(true);
        inventoryManager.AddWeapon(GunWeapon.GetComponent<Weapon>());
    }
    
    public void AddHardGunWeapon()
    {
        HardGunWeapon.SetActive(true);
        inventoryManager.AddWeapon(HardGunWeapon.GetComponent<Weapon>());
    }
    
    public void AddMeleeCircleWeapon()
    {
        MeleeCircleWeapon.SetActive(true);
        inventoryManager.AddWeapon(MeleeCircleWeapon.GetComponent<Weapon>());
    }
    
    public void AddMeleeBoxWeapon()
    {
        MeleeBoxWeapon.SetActive(true);
        inventoryManager.AddWeapon(MeleeBoxWeapon.GetComponent<Weapon>());
    }
    
    public void AddSkyFallWeapon()
    {
        SkyFallWeapon.SetActive(true);
        inventoryManager.AddWeapon(SkyFallWeapon.GetComponent<Weapon>());
    }
}
