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

    [SerializeField] private GameManager gameManager;
    
    private void Start()
    {
        Exp.OnExpCollect += IncreaseXP;
    }

    private void IncreaseXP(float amount)
    {
        _curXP += amount * PlayerStats.instance.XPMultiplier;
        int levelsToUp = 0;
        while (_curXP >= XPToNextLvl && !gameManager.ChoosingUpgrade)
        {
            // Debug.Log("LVL UP");
            _curLvl += 1;
            levelText.text = $"LEVEL {_curLvl}";
            _curXP -= XPToNextLvl;
            XPToNextLvl *= 1.2f;
            levelsToUp++;
        }
        levelBar.fillAmount = _curXP / XPToNextLvl;
        if (levelsToUp > 0)
        {
            gameManager.StartLevelUp(levelsToUp);
        }

    }

    public int GetCurrentLevel()
    {
        return _curLvl;
    }
}
