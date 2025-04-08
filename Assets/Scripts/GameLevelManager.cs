using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    [SerializeField] private int MaxLevel = 10;

    public int GetCurrentLevel()
    {
        if (PlayerPrefs.HasKey("CurrentLevel")) return PlayerPrefs.GetInt("CurrentLevel");
        PlayerPrefs.SetInt("CurrentLevel", 1);
        return 1;
    }

    public void LevelUp()
    {
        int currentLevel = GetCurrentLevel();
        if (currentLevel < MaxLevel)
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}
