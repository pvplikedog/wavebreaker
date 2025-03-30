using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<Image> passiveUISlots = new List<Image>(6);
    
    public int currentWeaponIndex = 0;
    public int currentPassiveIndex = 0;

    public void AddWeapon(Weapon weapon)
    {
        weaponUISlots[currentWeaponIndex].sprite = weapon.Icon;
        weaponUISlots[currentWeaponIndex].color = new Color(1f, 1f, 1f, 1f);
        currentWeaponIndex++;
    }
    
    public void AddPassive(UpgradeOptionSO passive)
    {
        passiveUISlots[currentPassiveIndex].sprite = passive.Icon;
        passiveUISlots[currentPassiveIndex].color = new Color(1f, 1f, 1f, 1f);
        currentPassiveIndex++;
    }
}
