using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    public enum Goal
    {
        SurviveTime,
        EnemiesKill,
        GetLevel
    }
    
    [SerializeField] private Goal goalType;
    [SerializeField] private int targetSurviveTime;
    [SerializeField] private int targetEnemies;
    [SerializeField] private int targetLevel;
    
    [SerializeField] private string goalText;
    
    [SerializeField] private TMP_Text goalTextUI;
    
    private int surviveTime;
    private int enemiesKilled;
    private int level;
    
    public static LevelGoal instance;

    private void Awake()
    {
        instance = this;
        goalTextUI.text = "LEVEL GOAL:\n" + goalText;
    }

    private void Update()
    {
        switch (goalType)
        {
            case Goal.SurviveTime:
                if (surviveTime >= targetSurviveTime)
                {
                    LevelFinish();
                }
                break;
            case Goal.EnemiesKill:
                if (enemiesKilled >= targetEnemies)
                {
                    LevelFinish();
                }
                break;
            case Goal.GetLevel:
                if (level >= targetLevel)
                {
                    LevelFinish();
                }
                break;
        }
    }

    private void LevelFinish()
    {
        Debug.Log("Level finished");
    }

    public void UpdateSurviveTime(float time)
    {
        surviveTime = (int)math.floor(time);
    }
    
    public void UpdateEnemiesKilled()
    {
        enemiesKilled++;
    }
    
    public void UpdateLevel(int level)
    {
        this.level = level;
    }

    public object GetEnemiesKilled()
    {
        return enemiesKilled;
    }
}
