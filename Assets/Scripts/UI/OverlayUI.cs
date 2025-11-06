using System;
using UnityEngine;
using UnityEngine.UIElements;

public class OverlayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument overlayUI;
    [SerializeField] private PlayerHealth playerHealth; 
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private PlayerStatsManager playerStatsManager;

    // UI Elements
    private Label 
        waveLabel,
        scoreLabel,
        highscoreLabel,
        levelLabel,
        moneyLabel;
    private ProgressBar 
        xpBar,
        hpBar;

    private void Awake()
    {
        var root = overlayUI.rootVisualElement;
        waveLabel = root.Q<Label>("waveLabel");
        scoreLabel = root.Q<Label>("scoreLabel");
        highscoreLabel = root.Q<Label>("highscoreLabel");
        levelLabel = root.Q<Label>("levelLabel");
        moneyLabel = root.Q<Label>("moneyLabel");
        xpBar = root.Q<ProgressBar>("xpBar");
        hpBar = root.Q<ProgressBar>("hpBar");

        playerHealth.OnHealthChanged += UpdateHealthBar;
        levelManager.OnXPChanged += UpdateXPBar;
        levelManager.OnLevelChanged += UpdateLevelUI;
        levelManager.OnScoreChanged += UpdateScoreUI;
        levelManager.OnMoneyChanged += UpdateMoneyUI;
        waveManager.OnWaveChanged += UpdateWaveUI;
        playerStatsManager.OnStatChanged += UpdateStats;
    }

    private void UpdateStats()
    {
        Debug.Log("muss noch");
    }

    private void Start()
    {
        UpdateAllUIs();
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
        levelManager.OnXPChanged -= UpdateXPBar;
        levelManager.OnLevelChanged -= UpdateLevelUI;
        levelManager.OnMoneyChanged -= UpdateMoneyUI;
        waveManager.OnWaveChanged -= UpdateWaveUI;
        levelManager.OnScoreChanged -= UpdateScoreUI;
    }

    // ======== Update-Methoden ========

    private void UpdateHealthBar(float current, float max)
    {
        if (hpBar == null)
            return;
        hpBar.value = (current / max) * 100f;
        hpBar.title = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }

    private void UpdateXPBar(float current, float max)
    {
        if (xpBar == null)
            return;
        xpBar.value = (current / max) * 100f;
        xpBar.title = $"{Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
    }

    private void UpdateLevelUI()
    {
        if (levelLabel == null)
            return;
        levelLabel.text = $"Level: {levelManager.currentLevel}";
    }

    private void UpdateMoneyUI(int money)
    {
        if (moneyLabel == null)
            return;
        moneyLabel.text = $"$ {money}";
    }

    private void UpdateWaveUI()
    {
        if (waveLabel == null)
            return;
        waveLabel.text = $"Wave {waveManager.currentWaveIndex + 1}";
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreLabel == null)
            return;
        scoreLabel.text = $"Score: {score}";
        if (highscoreLabel == null)
            return;
        highscoreLabel.text = $"Highscore: {levelManager.playerHighscore}";
    }

    private void UpdateAllUIs()
    {
        UpdateHealthBar(200, 200);
        UpdateLevelUI();
        UpdateWaveUI();
        UpdateMoneyUI(0);
        UpdateScoreUI(0);
        UpdateXPBar(0, 100);
    }
}
