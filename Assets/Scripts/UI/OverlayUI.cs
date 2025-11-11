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
        levelLabel,
        moneyLabel,
        lowHealthLabel,
        highHealthLabel,
        remainingTime;

    private void Awake()
    {
        var root = overlayUI.rootVisualElement;
        waveLabel = root.Q<Label>("waveNumber");
        scoreLabel = root.Q<Label>("scoreLabel");
        levelLabel = root.Q<Label>("levelLabel");
        moneyLabel = root.Q<Label>("moneyLabel");
        lowHealthLabel = root.Q<Label>("lowHealthLabel");
        highHealthLabel = root.Q<Label>("highHealthLabel");
        remainingTime = root.Q<Label>("remainingTime");

        playerHealth.OnHealthChanged += UpdateHealthBar;
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

    private void Update()
    {
        TimerUI();
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
        levelManager.OnLevelChanged -= UpdateLevelUI;
        levelManager.OnMoneyChanged -= UpdateMoneyUI;
        waveManager.OnWaveChanged -= UpdateWaveUI;
        levelManager.OnScoreChanged -= UpdateScoreUI;
    }

    // ======== Update-Methoden ========

    private void UpdateHealthBar(float current, float max)
    {
        lowHealthLabel.text = $"{Mathf.RoundToInt(current)}";
        highHealthLabel.text = $"{Mathf.RoundToInt(max)}";
    }

    private void UpdateLevelUI()
    {
        if (levelLabel == null)
            return;
        levelLabel.text = $"{levelManager.currentLevel}";
    }

    private void UpdateMoneyUI(int money)
    {
        if (moneyLabel == null)
            return;
        moneyLabel.text = $"{money} $";
    }

    private void UpdateWaveUI()
    {
        if (waveLabel == null)
            return;
        waveLabel.text = $"{waveManager.currentWaveIndex + 1}";
    }

    private void TimerUI()
    {
        if (remainingTime != null)
        {
            float timeLeft = waveManager.GetRemainingTime();
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            remainingTime.text = $"{minutes:00}:{seconds:00}";
        }
    }
 

private void UpdateScoreUI(int score)
    {
        if (scoreLabel == null)
            return;
        scoreLabel.text = $"{score}";   
    }

    private void UpdateAllUIs()
    {
        UpdateHealthBar(200, 200);
        UpdateLevelUI();
        UpdateWaveUI();
        UpdateMoneyUI(0);
        UpdateScoreUI(0);
    }

    public void ShowUI() => overlayUI.rootVisualElement.style.display = DisplayStyle.Flex;

    public void HideUI() => overlayUI.rootVisualElement.style.display = DisplayStyle.None;
}
