using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<Weapon> weaponSlots = new List<Weapon>(6);
    public List<Image> weaponUISlots = new List<Image>(6);
    
    private int currentWeaponIndex = 0;

    public void AddWeapon(Weapon weapon)
    {
        weaponSlots[currentWeaponIndex] = weapon;
        weaponUISlots[currentWeaponIndex].sprite = weapon.Icon;
        weaponUISlots[currentWeaponIndex].color = new Color(1f, 1f, 1f, 1f);
        currentWeaponIndex++;
    }
}
