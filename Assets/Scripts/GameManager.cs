using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GamePlay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private GameObject levelUpScreen;
    [SerializeField] private TMP_Text levelReachedDisplay;

    [HideInInspector] public bool IsGameOver;
    [HideInInspector] public bool ChoosingUpgrade;

    public List<Image> choosenWeaponsUI = new(6);
    public List<Image> choosenPassivessUI = new(6);

    [SerializeField] private TMP_Text timeSurvivedDisplay;

    [Header("Stopwatch")] [SerializeField] private float timeLimit; // Probably won't need.

    [SerializeField] private TMP_Text stopwatchDisplay;

    [SerializeField] private LevelUpManager levelUpManager;

    [Header("Damage Text Settings")] 
    public Canvas damageTextCanvas;
    public float textFontSize = 20f;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;
    


    [HideInInspector] public int levelsToUpdate;
    private float stopwatchTime;

    private void Awake()
    {
        Instance = this;
        DisableScreen();
    }

    private void Start()
    {
        // Probably will change to random weapon on start
        StartLevelUp(1);
    }

    private void Update()
    {
        CheckIfShouldLvlUp();
        switch (currentState)
        {
            case GameState.GamePlay:
                // Handle game play logic
                UpdateStopwatch();
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
            case GameState.LevelUp:
                if (!ChoosingUpgrade)
                {
                    // Debug.Log("LVL UP");
                    --levelsToUpdate;
                    ChoosingUpgrade = true;
                    Time.timeScale = 0;
                    levelUpScreen.SetActive(true);
                    levelUpManager.ConfigureUpgradeUI();
                }

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
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void DisableScreen()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
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

    public void AssignChoosenWeaponsAndPassives(List<Image> weapons, int weaponCount, List<Image> passives,
        int passiveCount)
    {
        for (var i = 0; i < weaponCount; i++)
            if (i < weapons.Count)
            {
                choosenWeaponsUI[i].sprite = weapons[i].sprite;
                choosenWeaponsUI[i].color = new Color(1f, 1f, 1f, 1f);
            }

        for (var i = 0; i < passiveCount; i++)
            if (i < passives.Count)
            {
                choosenPassivessUI[i].sprite = passives[i].sprite;
                choosenPassivessUI[i].color = new Color(1f, 1f, 1f, 1f);
            }
    }

    private void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();
    }

    private void UpdateStopwatchDisplay()
    {
        var minutes = (int)(stopwatchTime / 60);
        var seconds = (int)(stopwatchTime % 60);
        stopwatchDisplay.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }


    public void StartLevelUp(int levels)
    {
        levelsToUpdate = levels;
        ChangeState(GameState.LevelUp);
    }

    public void EndLevelUp()
    {
        ChoosingUpgrade = false;
        Time.timeScale = 1;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.GamePlay);
    }

    private void CheckIfShouldLvlUp()
    {
        if (levelsToUpdate > 0) ChangeState(GameState.LevelUp);
    }
    
    public static GameManager Instance { get; private set; }
    
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObject = new GameObject("Damage Floating Text");
        RectTransform rect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObject.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        // rect.position = referenceCamera.WorldToScreenPoint(target.position);
        Vector3 targetPosition = target.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.6f, 0.6f), 0);
        rect.position = referenceCamera.WorldToScreenPoint(targetPosition);

        Destroy(textObject, duration);
        
        textObject.transform.SetParent(Instance.damageTextCanvas.transform);
        
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        while (t < duration)
        {
            if (!textObject)
            {
                break;
            }
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);
            
            if(target) {
                yOffset += speed * Time.deltaTime;
                rect.position = referenceCamera.WorldToScreenPoint(targetPosition + new Vector3(0,yOffset));
            } else {
                // If target is dead, just pan up where the text is at.
                rect.position += new Vector3(0, speed * Time.deltaTime, 0);
            }
            
            yield return waitForEndOfFrame;
            t += Time.deltaTime;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!Instance.damageTextCanvas)
        {
            return;
        }

        if (!Instance.referenceCamera)
        {
            Instance.referenceCamera = Camera.main;
        }
        
        Instance.StartCoroutine(Instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
    }
}