using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }
    
    public Sprite Icon;
    public UpgradeOption UpgradeType;
    public int MaxLvl;
    public List<String> Names;
    public List<String> Descriptions;
}
