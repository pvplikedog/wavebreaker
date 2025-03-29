using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GamePlay,
        Paused,
        GameOver
    }

    public GameState currentState;
    public GameState previousState;
    
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private TMP_Text levelReachedDisplay;

    [HideInInspector] public bool IsGameOver = false;
    
    public List<Image> choosenWeaponsUI = new List<Image>(6);
    public List<Image> choosenPassivessUI = new List<Image>(6);

    private void Awake()
    {
        DisableScreen();
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.GamePlay:
                // Handle game play logic
                CheckForPauseAndResume();
                break;
            case GameState.Paused:
                // Handle paused logic
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!IsGameOver)
                {
                    IsGameOver = true;
                    DisplayResults();
                }
                // Handle game over logic
                break;
            default:
                Debug.LogWarning("State doesn't exits!");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            pauseScreen.SetActive(true); // Show the pause screen
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            DisableScreen();
            Time.timeScale = 1f; // Resume the game
        }
    }

    private void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    private void DisableScreen()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void DisplayResults()
    {
        resultScreen.SetActive(true);
    }
    
    public void AssignLevelReached(int level)
    {
        levelReachedDisplay.text = level.ToString();
    }
    
    public void AssignChoosenWeaponsAndPassives(List<Image> weapons, int weaponCount, List<Image> passives, int passiveCount)
    {
        for (int i = 0; i < weaponCount; i++)
        {
            if (i < weapons.Count)
            {
                choosenWeaponsUI[i].sprite = weapons[i].sprite;
                choosenWeaponsUI[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }

        for (int i = 0; i < passiveCount; i++)
        {
            if (i < passives.Count)
            {
                choosenPassivessUI[i].sprite = passives[i].sprite;
                choosenPassivessUI[i].color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
}
