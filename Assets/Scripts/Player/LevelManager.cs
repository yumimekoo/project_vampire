using System;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] public int currentLevel = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 100f;

    [Header("Level Settings")]
    [SerializeField] private int playerMoney = 0;
    [SerializeField] private int playerScore = 0;
    [SerializeField] public int playerHighscore = 0;

    [Header("Level Settings")]
    [SerializeField] private float xpGrowthFactor = 1.25f;
    [SerializeField] private float enemyDifficultyMultiplier = 1.1f;

    //public System.Action<int> OnLevelUp;
    //public System.Action<int> OnMoneyChanged;
    //public System.Action<int> OnScoreChanged;

    public event System.Action<float, float> OnXPChanged;
    public event System.Action OnLevelChanged;
    public event System.Action<int> OnMoneyChanged;
    public event System.Action<int> OnScoreChanged;

    // -----------
    // XP SYSTEM 
    // -----------
    public void Awake()
    {
        HighScoreUpdate();
        OnScoreChanged?.Invoke(playerScore);
    }
    public void AddXP(float amount)
    {
        currentXP += amount;

        // hier war "while" warum auch immer
        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    public void LevelUp()
    {
        currentLevel++;
        xpToNextLevel *= xpGrowthFactor;

        // TODO levelup animation screen 
        Debug.Log($"LEVEL UP TO: {currentLevel}");
        OnLevelChanged?.Invoke();
    }

    // -----------
    // ECONOMY SYSTEM
    // -----------

    public void AddMoney(int  amount)
    {
        playerMoney += amount;
        OnMoneyChanged?.Invoke(playerMoney);
    }

    public bool TrySpendMoney(int cost)
    {
        if(playerMoney >= cost)
        {
            playerMoney -= cost;
            OnMoneyChanged?.Invoke(playerMoney);
            return true;
        }

        return false;
    }

    // -----------
    // SCORE SYSTEM
    // -----------

    public void AddScore(int amount)
    {
        playerScore += amount;
        HighScoreUpdate();
        OnScoreChanged?.Invoke(playerScore);
    }

    private void HighScoreUpdate()
    {
        if(PlayerPrefs.HasKey("highscore"))
        {
            playerHighscore = PlayerPrefs.GetInt("highscore");
            if(playerScore > playerHighscore)
            {
                playerHighscore = playerScore;
                PlayerPrefs.SetInt("highscore", playerHighscore);
            }
        }
        else
        {
            playerHighscore = playerScore;
            PlayerPrefs.SetInt("highscore", playerHighscore);
        }
    }

    public void ResetScore()
    {
        playerScore = 0;
        OnScoreChanged?.Invoke(playerScore);
    }

    // -----------
    // ACCESSORS
    // -----------

    public int GetLevel() => currentLevel;
    public float GetEnemyDifficultyMultiplier() => Mathf.Pow(enemyDifficultyMultiplier, currentLevel - 1);
    public int GetScore() => playerScore;
    public int GetMoney() => playerMoney;
    public float GetXPPercent() => currentXP / xpToNextLevel;
}
