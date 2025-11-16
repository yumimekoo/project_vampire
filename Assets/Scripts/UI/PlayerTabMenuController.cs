using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerTabMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument tabMenu;
    [SerializeField] private PlayerStatsManager stats;
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
        labelDefense,
        labelLifeSteal;

    private Label
        labelMoveSpeed,
        labelMoveSpeedP,
        labelDashDistance,
        labelDashCooldown,
        labelDashCooldownP,
        labelDashes;
    private bool isTabHeld = false;
    private Coroutine timeScaleCoroutine;

    void Start()
    {
        var root = tabMenu.rootVisualElement;

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
        labelLifeSteal = root.Q<Label>("labelLifeSteal");

        labelMoveSpeed = root.Q<Label>("labelMoveSpeed");
        labelMoveSpeedP = root.Q<Label>("labelMoveSpeedP");
        labelDashDistance = root.Q<Label>("labelDashDistance");
        labelDashCooldown = root.Q<Label>("labelDashCooldown");
        labelDashCooldownP = root.Q<Label>("labelDashCooldownP");
        labelDashes = root.Q<Label>("labelDashes");

        root.style.display = DisplayStyle.None;
        AssignValues();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!isTabHeld)
                if (IngameMusic.Instance != null)
                    IngameMusic.Instance.PitchDown(0.3f);
            OpenTabMenu();
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(isTabHeld)
                if (IngameMusic.Instance != null)
                    IngameMusic.Instance.PitchUp(0.3f);
            CloseTabMenu();
        }
    }

    private void OpenTabMenu()
    {
        
        if(GameState.inShop || GameState.inPauseMenu)
        {
            return;
        }
        AssignValues();
        GameState.inTabPauseMenu = true;
        isTabHeld = true;

        tabMenu.rootVisualElement.style.display = DisplayStyle.Flex;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 0f));
    }

    private void CloseTabMenu()
    {
        isTabHeld = false;
        tabMenu.rootVisualElement.style.display = DisplayStyle.None;

        if(timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(AdjustTimeScale(Time.timeScale, 1f));
        
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
        labelLifeSteal.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.LifeSteal) * 100f)}%";
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
        float duration = 0.6f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(start, end, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = end;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (GameState.inTabPauseMenu && !isTabHeld)
        {
            GameState.inTabPauseMenu = false;
        }
    }

    public void HideUI()
    {
        GameState.inTabPauseMenu = false;
        isTabHeld = false;
        tabMenu.rootVisualElement.style.display = DisplayStyle.None;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        Time.timeScale = 1f;
    }


}
