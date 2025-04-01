using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeOptionSO", menuName = "Scriptable Objects/UpgradeOptionSO")]
public class UpgradeOptionSO : ScriptableObject
{
    public enum UpgradeOption
    {
        Gun,
        HardGun,
        MeleeCircle,
        MeleeBox,
        SkyFall,
        Heal,
        healthPassive,
        regenPassive,
        speedPassive,
        damagePassive,
        xpPassive,
        damageReducePassive,
        StatueHealthPassive,
        StatueRegenPassive,
        StatueDamageReducePassive,
        StatueFireRatePassive,
        StatueGun,
        MassDestruct,
        FreezingWeapon,
        AuraWeapon,
        VerticalWeapon
    }

    public Sprite Icon;
    public UpgradeOption UpgradeType;
    public int MaxLvl;
    public List<string> Names;
    public List<string> Descriptions;
}