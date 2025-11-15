using DG.Tweening;
using System;
using System.Collections;
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
    [SerializeField] private UIDocument levelAndWave;
    [SerializeField] private Sprite[] sprites;
    private int frames = 8;

    // UI Elements
    private Label 
        waveLabel,
        scoreLabel,
        levelLabel,
        moneyLabel,
        lowHealthLabel,
        highHealthLabel,
        remainingTime,
        waveUpLabel;


    private VisualElement 
        waveUp,
        levelUp;


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

        waveUp = levelAndWave.rootVisualElement.Q<VisualElement>("waveVisual");
        levelUp = levelAndWave.rootVisualElement.Q<VisualElement>("levelUpVisual");
        waveUpLabel = levelAndWave.rootVisualElement.Q<Label>("waveLabel");

        waveUp.style.opacity = 0;
        levelUp.style.opacity = 0;

        playerHealth.OnHealthChanged += UpdateHealthBar;
        levelManager.OnLevelChanged += UpdateLevelUI;
        levelManager.OnScoreChanged += UpdateScoreUI;
        levelManager.OnMoneyChanged += UpdateMoneyUI;
        waveManager.OnWaveChanged += UpdateWaveUI;
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
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        if (!GameState.isTutroial)
        {
            Sequence s = DOTween.Sequence()
            .Append(
                DOTween.To(() => (float) levelUp.style.opacity.value,
                           x => levelUp.style.opacity = x,
                           1f,                  // Ziel=1
                           0.5f                 // Fade-In Dauer
                )
            )
            .AppendInterval(1f)                 // 1 Sekunde warten
            .Append(
                DOTween.To(() => (float) levelUp.style.opacity.value,
                           x => levelUp.style.opacity = x,
                           0f,                  // Ziel=0
                           0.5f                 // Fade-Out Dauer
                )
            )
            .SetUpdate(true);
        }

        overlayUI.rootVisualElement.style.backgroundImage = new StyleBackground();

        for (int i = 0; i < sprites.Length; i++)
        {
            overlayUI.rootVisualElement.style.backgroundImage = new StyleBackground(sprites[i]);
            yield return new WaitForSeconds(1f / frames);
        }
    }

    public IEnumerator PlayWaveAnimation()
    {
        if (!GameState.isTutroial)
        {
            Sequence s = DOTween.Sequence()
        .Append(
            DOTween.To(() => (float) waveUp.style.opacity.value,
                       x => waveUp.style.opacity = x,
                       1f,                  // Ziel=1
                       0.5f                 // Fade-In Dauer
            )
        )
        .AppendInterval(1f)                 // 1 Sekunde warten
        .Append(
            DOTween.To(() => (float) waveUp.style.opacity.value,
                       x => waveUp.style.opacity = x,
                       0f,                  // Ziel=0
                       0.5f                 // Fade-Out Dauer
            )
        )
        .SetUpdate(true);
            yield return s.WaitForCompletion();
        }
           
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
        waveUpLabel.text = $"Wave {waveManager.currentWaveIndex + 1}";
        StartCoroutine(PlayWaveAnimation());
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
