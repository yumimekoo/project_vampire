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
        labelDefense;
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
                OpenTabMenu();
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            if(isTabHeld)
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
        GameState.inTabPauseMenu = false;
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
        labelMaxHealth.text = $"{Mathf.RoundToInt(stats.GetStat(StatType.MaxHealth) * 10) / 10}";
        labelAttackDamage.text = $"{Mathf.Round(stats.GetStat(StatType.AttackDamage) * 10) / 10}";
        labelAttackSpeed.text = $"{Mathf.Round(stats.GetStat(StatType.AttackSpeed) * 10) / 10}";
        labelBulletSpread.text = $"{Mathf.Round(stats.GetStat(StatType.BulletSpread) * 10) / 10}";
        labelBulletDistance.text = $"{Mathf.Round(stats.GetStat(StatType.BulletDistance) * 10) / 10}";
        labelBulletSpeed.text = $"{Mathf.Round(stats.GetStat(StatType.BulletSpeed) * 10) / 10}";
        labelDefense.text = $"{Mathf.RoundToInt(stats.GetStat(StatType.Defense) * 10) / 10}";
        labelMoveSpeed.text = $"{Mathf.Round(stats.GetStat(StatType.MoveSpeed) * 10) / 10}";
        labelDashDistance.text = $"{Mathf.Round(stats.GetStat(StatType.DashDistance) * 10) / 10}";
        labelDashCooldown.text = $"{Mathf.Round(stats.GetStat(StatType.DashCooldown) * 10) / 10}";
        labelDashes.text = $"{Mathf.RoundToInt(stats.GetStat(StatType.Dashes) * 10) / 10}";

        labelMaxHealthP.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.MaxHealthPercent) * 20) / 20}";
        labelAttackDamageP.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.AttackPercent) * 20) / 20}";
        labelAttackSpeedP.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.AttackSpeedPercent) * 20) / 20}";
        labelMoveSpeedP.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.MovePercent) * 20) / 20}";
        labelDashCooldownP.text = $"{Mathf.Round(stats.GetStatMulti(StatMulti.DashCooldownPercent) * 20) / 20}";

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
    }



}
