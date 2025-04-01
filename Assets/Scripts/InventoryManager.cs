using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<Image> weaponUISlots = new(6);
    public List<Image> passiveUISlots = new(6);

    public int currentWeaponIndex;
    public int currentPassiveIndex;

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