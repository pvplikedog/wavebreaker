using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuLevelText : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameLevelManager gameLevelManager;
    
    [SerializeField] private List<String> levelGoals = new List<String>();
    void Start()
    {
        Time.timeScale = 1;
        levelText.text = "LEVEL " + gameLevelManager.GetCurrentLevel().ToString() + "\n" + levelGoals[gameLevelManager.GetCurrentLevel() - 1];
    }
    
    void Update()
    {
        
    }
}
