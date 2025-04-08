using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetter : MonoBehaviour
{
    [SerializeField] private GameLevelManager gameLevelManager;
    
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("Level" + gameLevelManager.GetCurrentLevel().ToString());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
