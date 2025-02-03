using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float XPToNextLvl = 3f;
    private float _curXP = 0f;
    private int _curLvl = 1;
    
    [SerializeField] private Image levelBar;
    [SerializeField] private TMP_Text levelText;

    private void Start()
    {
        Exp.OnExpCollect += IncreaseXP;
    }

    private void IncreaseXP(float amount)
    {
        _curXP += amount;
        while (_curXP >= XPToNextLvl)
        {
            Debug.Log("LVL UP");
            _curLvl += 1;
            levelText.text = $"LEVEL {_curLvl}";
            _curXP -= XPToNextLvl;
            XPToNextLvl *= 1.2f;
        }
        levelBar.fillAmount = _curXP / XPToNextLvl;
    }
}
