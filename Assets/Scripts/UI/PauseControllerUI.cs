using DG.Tweening;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseControllerUI : MonoBehaviour
{
    [SerializeField] private UIDocument pauseUI;
    [SerializeField] private OverlayUI overlayUI;
    [SerializeField] private UIDocument loadingUI;
    [SerializeField] private HealthBarUnderlayUI underlayUI;
    [SerializeField] private PlayerStatsManager stats;

    private Button
        continueButton,
        newGameButton,
        exitButton;
    private Coroutine timeScaleCoroutine;

    private Label
        labelMaxHealth,
        labelMaxHealthP,
        labelAttackDamage,
        labelAttackDamageP,
        labelAttackSpeed,
        labelAttackSpeedP,
        labelBulletSpeed,
        labelBulletDistance,
        labelBulletSpread,
        labelDefense;

    private Label
        labelMoveSpeed,
        labelMoveSpeedP,
        labelDashDistance,
        labelDashCooldown,
        labelDashCooldownP,
        labelDashes;

    void Start()
    {
        var root = pauseUI.rootVisualElement;
        continueButton = root.Q<Button>("buttonContinue");
        newGameButton = root.Q<Button>("buttonNewGame");
        exitButton = root.Q<Button>("buttonExit");

        continueButton.clicked += ContinueButton;
        newGameButton.clicked += NewGameButton;
        exitButton.clicked += ExitButton;

        labelMaxHealth = root.Q<Label>("labelMaxHealth");
        labelMaxHealthP = root.Q<Label>("labelMaxHealthP");
        labelAttackDamage = root.Q<Label>("labelAttackDamage");
        labelAttackDamageP = root.Q<Label>("labelAttackDamageP");
        labelAttackSpeed = root.Q<Label>("labelAttackSpeed");
        labelAttackSpeedP = root.Q<Label>("labelAttackSpeedP");
        labelBulletSpeed = root.Q<Label>("labelBulletSpeed");
        labelBulletDistance = root.Q<Label>("labelBulletDistance");
        labelBulletSpread = root.Q<Label>("labelBulletSpread");
        labelDefense = root.Q<Label>("labelDefense");

        labelMoveSpeed = root.Q<Label>("labelMoveSpeed");
        labelMoveSpeedP = root.Q<Label>("labelMoveSpeedP");
        labelDashDistance = root.Q<Label>("labelDashDistance");
        labelDashCooldown = root.Q<Label>("labelDashCooldown");
        labelDashCooldownP = root.Q<Label>("labelDashCooldownP");
        labelDashes = root.Q<Label>("labelDashes");

        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void ExitButton()
    {
        GameState.inPauseMenu = false;
        Time.timeScale = 1f;
        loadingUI.rootVisualElement.style.opacity = 0f;
        loadingUI.rootVisualElement.style.display = DisplayStyle.Flex;
        StartCoroutine(LoadScene("MainMenu"));
    }

    private void NewGameButton()
    { 
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.inPauseMenu = false;
        Time.timeScale = 1f;
        loadingUI.rootVisualElement.style.opacity = 0f;
        loadingUI.rootVisualElement.style.display = DisplayStyle.Flex;
        StartCoroutine(LoadScene("SampleScene"));
    }

    public IEnumerator LoadScene(string name)
    {
        DOTween.To(
            () => (float) loadingUI.rootVisualElement.resolvedStyle.opacity,
            x => loadingUI.rootVisualElement.style.opacity = x,
            1f, 1f
            ).OnComplete(() =>
            {
                GameState.Reset();
                SceneManager.LoadSceneAsync(name);
            });
        yield return null;
    }

    private void ContinueButton()
    {
        overlayUI.ShowUI();
        underlayUI.ShowUI();
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.inPauseMenu = false;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 1f));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !GameState.inShop && !GameState.inTabPauseMenu && !GameState.isDead)
        {
            if (!GameState.inPauseMenu)
            {

                pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
                pauseUI.rootVisualElement.style.opacity = 0f;
                DOTween.To(
                    () => (float) pauseUI.rootVisualElement.resolvedStyle.opacity,
                    x => pauseUI.rootVisualElement.style.opacity = x,
                    1f, 0.2f
                    );
                overlayUI.HideUI();
                underlayUI.HideUI();
                AssignValues();
                GameState.inPauseMenu = true;
                if (timeScaleCoroutine != null)
                {
                    StopCoroutine(timeScaleCoroutine);
                }
                timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 0f));
            }
            else if (GameState.inPauseMenu) {
                ContinueButton();
            }
        }
    }

    private void AssignValues()
    {
        // --- MAX HEALTH ---
        float mhFlat = stats.GetStat(StatType.MaxHealth);
        float mhMulti = stats.GetStatMulti(StatMulti.MaxHealthPercent);
        float mhFinal = mhFlat * mhMulti;
        labelMaxHealth.text = $"{Mathf.Round(mhFinal)}";
        labelMaxHealthP.text = FormatPercent(mhFlat, mhMulti);

        // --- ATTACK DAMAGE ---
        float adFlat = stats.GetStat(StatType.AttackDamage);
        float adMulti = stats.GetStatMulti(StatMulti.AttackPercent);
        float adFinal = adFlat * adMulti;
        labelAttackDamage.text = $"{Mathf.Round(adFinal * 10) / 10}";
        labelAttackDamageP.text = FormatPercent(adFlat, adMulti);

        // --- ATTACK SPEED ---
        float asFlat = stats.GetStat(StatType.AttackSpeed);
        float asMulti = stats.GetStatMulti(StatMulti.AttackSpeedPercent);
        float asFinal = asFlat * asMulti;
        labelAttackSpeed.text = $"{Mathf.Round(asFinal * 10) / 10}";
        labelAttackSpeedP.text = FormatPercent(asFlat, asMulti);

        // --- MOVE SPEED ---
        float msFlat = stats.GetStat(StatType.MoveSpeed);
        float msMulti = stats.GetStatMulti(StatMulti.MovePercent);
        float msFinal = msFlat * msMulti;
        labelMoveSpeed.text = $"{Mathf.Round(msFinal * 10) / 10}";
        labelMoveSpeedP.text = FormatPercent(msFlat, msMulti);

        // --- DASH COOLDOWN ---
        float dcFlat = stats.GetStat(StatType.DashCooldown);
        float dcMulti = stats.GetStatMulti(StatMulti.DashCooldownPercent);
        float dcFinal = dcFlat * dcMulti;
        labelDashCooldown.text = $"{Mathf.Round(dcFinal * 10) / 10}";
        labelDashCooldownP.text = FormatPercent(dcFlat, dcMulti);

        // --- VALUES OHNE MULTI ---
        labelBulletSpread.text = $"{Mathf.Round(stats.GetStat(StatType.BulletSpread) * 10) / 10}";
        labelBulletDistance.text = $"{Mathf.Round(stats.GetStat(StatType.BulletDistance) * 10) / 10}";
        labelBulletSpeed.text = $"{Mathf.Round(stats.GetStat(StatType.BulletSpeed) * 10) / 10}";
        labelDefense.text = $"{Mathf.Round(stats.GetStat(StatType.Defense) * 10) / 10}";
        labelDashDistance.text = $"{Mathf.Round(stats.GetStat(StatType.DashDistance) * 10) / 10}";
        labelDashes.text = $"{Mathf.RoundToInt(stats.GetStat(StatType.Dashes))}";
    }

    private string FormatPercent(float flat, float multi)
    {
        float percent = (multi - 1f) * 100f;

        string sign = percent >= 0 ? "+ " : " ";

        return $"{Mathf.Round(flat)} {sign}{percent:F0}%)";
    }

    private IEnumerator AdjustTimeScale(float start, float end)
    {
        float t = 0f;
        float duration = 0.2f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, end, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = end;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void HideUI()
    {
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        GameState.inPauseMenu = false;
        Time.timeScale = 1f;
    }
}
