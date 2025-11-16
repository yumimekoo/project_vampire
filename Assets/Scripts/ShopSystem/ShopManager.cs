using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private UIDocument shopUI;
    private int rerollCost = 0;
    private int rerollCount = 0;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private RarityScalingDataSO rarityScalingData;
    [SerializeField] private PlayerStatsManager playerStatsManager;
    [SerializeField] private PlayerHealth playerHealth;   
    [SerializeField] private SkillManager skillManager;
    [SerializeField] public WaveManager waveManager;
    [SerializeField] private PauseControllerUI pauseUI;
    [SerializeField] private PlayerTabMenuController tabUI;
    [SerializeField] private OverlayUI overlayUI;
    [SerializeField] private HealthBarUnderlayUI underlayUI;
    [SerializeField] private PlayerStatsManager stats;

    [SerializeField] private VisualTreeAsset commonItemTemplate;
    [SerializeField] private VisualTreeAsset uncommonItemTemplate;
    [SerializeField] private VisualTreeAsset rareItemTemplate;
    [SerializeField] private VisualTreeAsset mythicItemTemplate;
    [SerializeField] private VisualTreeAsset legendaryItemTemplate;
    [SerializeField] private VisualTreeAsset passiveSkillTemplate;

    [SerializeField] private AudioClip itemBought;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip buttonPress;
    [SerializeField] private AudioClip shopOpenSound;
    [SerializeField] private AudioClip shopCloseSound;
    [SerializeField] private AudioSource audioSource;

    private static readonly HashSet<StatType> InvertedFlatStats = new()
    {
        StatType.BulletSpread,
        StatType.DashCooldown
    };

    private static readonly HashSet<StatMulti> InvertedMultiStats = new()
    {
        StatMulti.DashCooldownPercent
    };

    private List<ShopItemDataSO> currentItems = new List<ShopItemDataSO>();
    private VisualElement root,
        itemContainer;

    private Button 
        rerollButton,
        nextWave;

    private Label 
        moneyLabel,
        rerollLabel;

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

    private Dictionary<ShopItemDataSO, VisualElement> itemElements = new();
    private void Awake()
    {
        root = shopUI.rootVisualElement;
        itemContainer = root.Q<VisualElement>("itemContainer");
        rerollButton = root.Q<Button>("rerollButton");
        rerollLabel = root.Q<Label>("rerollPrice");
        moneyLabel = root.Q<Label>("moneyLabel");
        nextWave = root.Q<Button>("nextWave");

        nextWave.clicked += () =>
        {
            
            rerollCost = 0;
            rerollCount = 0;
            waveManager.OnNextWaveButton();
            HideUI();
        };

        rerollButton.clicked += RerollShop;

        // STATS

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

        HideUI();
        
    }

    private void Start()
    {
        RollItems();
        UpdateMoneyDisplay();
        AssignValues();
    }

    public void RollItems()
    {
        
        rerollLabel.text = $"{rerollCost} $";
        itemContainer.Clear();
        currentItems.Clear();

        for (int i = 0; i < 3; i++)
        {
            var possibleItems = GetItemsForCurrentLevel();
            ShopItemDataSO randomItem = null;
            do
            {
                randomItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            }
            while (currentItems.Contains(randomItem));

            //var randomItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            currentItems.Add(randomItem);
            CreateItemUI(randomItem);
        }
    }

    private void CreateItemUI(ShopItemDataSO itemData)
    {
        var itemElement = GetTemplateForItem(itemData).CloneTree();
        var nameLabel = itemElement.Q<Label>("nameLabel");
        var descLabel = itemElement.Q<Label>("descLabel");
        var buyButton = itemElement.Q<Button>("buyButton");
        var iconElement = itemElement.Q<VisualElement>("iconElement");

        if(itemData.type == ItemType.StatUpgrade)
        {
            for (int i = 1; i <= 3; i++)
            {
                itemElement.Q<Label>($"pos{i}").style.display = DisplayStyle.None;
                itemElement.Q<Label>($"neg{i}").style.display = DisplayStyle.None;
            }
        }

       

        var allPositiveEffects = new List<(string text, float value)>();
        var allNegativeEffects = new List<(string text, float value)>();

        if (itemData.positiveEffects != null)
        {
            foreach (var e in itemData.positiveEffects)
                allPositiveEffects.Add(($"{GetVorzeichenPositive(e.statType)}{e.value} {GetEffectText(e.statType)}", e.value));
        }
        if (itemData.positiveEffectMultis != null)
        {
            foreach (var e in itemData.positiveEffectMultis)
                allPositiveEffects.Add(($"{GetVorzeichenPositive(e.statMulti)}{e.value * 100f}% {GetEffectText(e.statMulti)}", e.value));
        }

        if (itemData.negativeEffects != null)
        {
            foreach (var e in itemData.negativeEffects)
                allNegativeEffects.Add(($"{GetVorzeichenNegative(e.statType)}{e.value} {GetEffectText(e.statType)}", e.value));
        }

        if (itemData.negativeEffectMultis != null)
        {
            foreach (var e in itemData.negativeEffectMultis)
                allNegativeEffects.Add(($"{GetVorzeichenNegative(e.statMulti)}{e.value * 100f}% {GetEffectText(e.statMulti)}", e.value));
        }

        for (int i = 0; i < allPositiveEffects.Count && i < 3; i++)
        {
            var posElement = itemElement.Q<Label>($"pos{i + 1}");
            posElement.style.display = DisplayStyle.Flex;
            posElement.text = allPositiveEffects[i].text;
        }

        for (int i = 0; i < allNegativeEffects.Count && i < 3; i++)
        {
            var negElement = itemElement.Q<Label>($"neg{i + 1}");
            negElement.style.display = DisplayStyle.Flex;
            negElement.text = allNegativeEffects[i].text;
        }


        if (itemData.icon != null)
        {
            
            iconElement.style.backgroundImage = new StyleBackground(itemData.icon.texture);
        }

        nameLabel.text = itemData.itemName;
        descLabel.text = itemData.description;
        buyButton.text = $"{itemData.basePrice}$";
        buyButton.clicked += () => BuyItem(itemData);


        itemContainer.Add(itemElement);
        itemElements[itemData] = itemElement;
    }

    private string GetEffectText(StatType statType)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                return "Max Health";
            case StatType.AttackDamage:
                return "Attack Damage";
            case StatType.AttackSpeed:
                return "Attack Speed";
            case StatType.BulletSpeed:
                return "Bullet Speed";
            case StatType.BulletDistance:
                return "Bullet Distance";
            case StatType.BulletSpread:
                return "Bullet Spread";
            case StatType.Defense:
                return "Defense";
            case StatType.MoveSpeed:
                return "Move Speed";
            case StatType.DashDistance:
                return "Dash Distance";
            case StatType.DashCooldown:
                return "Dash Cooldown";
            case StatType.Dashes:
                return "Dashes";
            default:
                return statType.ToString();
        }
    }

    private string GetEffectText(StatMulti statMulti)
    {
        switch (statMulti)
        {
            case StatMulti.MaxHealthPercent:
                return "Max Health";
            case StatMulti.AttackPercent:
                return "Attack Damage";
            case StatMulti.AttackSpeedPercent:
                return "Attack Speed";
            case StatMulti.MovePercent:
                return "Move Speed";
            case StatMulti.DashCooldownPercent:
                return "Dash Cooldown";
            case StatMulti.LifeSteal:
                return "Life Steal";
            default:
                return statMulti.ToString();
        }
    }

    private string GetVorzeichenPositive(StatType statType)
    {
        switch (statType)
        {
            case StatType.DashCooldown:
            case StatType.BulletSpread:
                return "-";
            default:
                return "+";
        }
    }
    private string GetVorzeichenPositive(StatMulti statMulti)
    {
        switch (statMulti)
        {
            case StatMulti.DashCooldownPercent:
                return "-";
            default:
                return "+";
        }

    }
    private string GetVorzeichenNegative(StatType statType)
    {
        switch (statType)
        {
            case StatType.DashCooldown:
            case StatType.BulletSpread:
                return "+";
            default:
                return "-";
        }
    }
    private string GetVorzeichenNegative(StatMulti statMulti)
    {
        switch (statMulti)
        {
            case StatMulti.DashCooldownPercent:
                return "+";
            default:
                return "-";
        }
    }




    private VisualTreeAsset GetTemplateForItem(ShopItemDataSO item)
    {
        if (item.type == ItemType.PassiveSkill || item.type == ItemType.ActiveSkill)
        {
            return passiveSkillTemplate;
        }

        return item.rarity switch
        {
            ItemRarity.Common => commonItemTemplate,
            ItemRarity.Uncommon => uncommonItemTemplate,
            ItemRarity.Rare => rareItemTemplate,
            ItemRarity.Mythic => mythicItemTemplate,
            ItemRarity.Legendary => legendaryItemTemplate,
            _ => commonItemTemplate,
        };
    }
    private void BuyItem(ShopItemDataSO item)
    {
        if (levelManager.TrySpendMoney(item.basePrice))
        {
            audioSource.PlayOneShot(itemBought);
            ApplyItemEffect(item);
            playerHealth.UpdateMaxHealth();
            if (itemElements.TryGetValue(item, out var element))
            {
                element.AddToClassList("shop-item-disabled");
                var button = element.Q<Button>();
                button.SetEnabled(false);
            }
            AssignValues();
        }
        else
        {
            audioSource.PlayOneShot(errorSound);
            Debug.Log("Not Enough Money");
        }
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        moneyLabel.text = $"{levelManager.GetMoney()} $";
    }

    private void ApplyItemEffect(ShopItemDataSO item)
    {
        switch (item.type)
        {
            case ItemType.StatUpgrade:
                foreach (var effect in item.positiveEffects)
                {
                    if (InvertedFlatStats.Contains(effect.statType))
                        playerStatsManager.ReduceFlatStat(effect.statType, effect.value);
                    else
                        playerStatsManager.AddFlatStat(effect.statType, effect.value);
                }
                foreach (var effect in item.negativeEffects)
                {
                    if (InvertedFlatStats.Contains(effect.statType))
                        playerStatsManager.AddFlatStat(effect.statType, effect.value);
                    else
                        playerStatsManager.ReduceFlatStat(effect.statType, effect.value);
                }
                foreach (var effect in item.positiveEffectMultis)
                {
                    if(InvertedMultiStats.Contains(effect.statMulti))
                        playerStatsManager.ReducePercentStat(effect.statMulti, effect.value);
                    else
                        playerStatsManager.AddPercentStat(effect.statMulti, effect.value);
                }
                foreach (var effect in item.negativeEffectMultis)
                {
                    if(InvertedMultiStats.Contains(effect.statMulti))
                        playerStatsManager.AddPercentStat(effect.statMulti, effect.value);
                    else
                        playerStatsManager.ReducePercentStat(effect.statMulti, effect.value);
                }
                break;
            case ItemType.ActiveSkill:
                // TODO Active Skill Implementation
                Debug.Log("Active Skill Purchased - Implementation Pending");
                break;
            case ItemType.PassiveSkill:
                skillManager.ApplyItem(item);
                Debug.Log("Passive Skill Purchased - Implementation Pending");
                break;
            default:
                Debug.LogWarning("Unknown item type");
                break;
        }
        Debug.Log($"Bought {item.itemName}");
    }

    private void RerollShop()
    {
        if (levelManager.TrySpendMoney(rerollCost))
        {
            audioSource.PlayOneShot(buttonPress);
            UpdateMoneyDisplay();
            rerollCount++;
            rerollCost += 5 * (rerollCount + 1);
            rerollLabel.text = $"{rerollCost} $";
            RollItems();
        }
        else
        {
            audioSource.PlayOneShot(errorSound);
            Debug.Log("Not enough Money");
        }
    }

    private List<ShopItemDataSO> GetItemsForCurrentLevel()
    {
        ItemRarity rarity = rarityScalingData.GetRarityForLevel(levelManager.GetLevel());
        var allItems = Resources.LoadAll<ShopItemDataSO>("ShopItems");
        return allItems.Where(i => i.rarity == rarity).ToList();
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

    public void HideUI()
    {
        audioSource.PlayOneShot(shopCloseSound);
        overlayUI.ShowUI();
        underlayUI.ShowUI();
        GameState.inShop = false;
        shopUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void ShowUI()
    {
        audioSource.PlayOneShot(shopOpenSound);
        UpdateMoneyDisplay();
        overlayUI.HideUI();
        underlayUI.HideUI();
        tabUI.HideUI();
        pauseUI.HideUI();
        GameState.inShop = true;
        shopUI.rootVisualElement.style.display = DisplayStyle.Flex;
    }


    [ContextMenu("Test Rarity Distribution")]
    private void TestRarityDistribution()
    {
        int playerLevel = 25; // Oder beliebiger Test-Level
        int testRuns = 10000; // Anzahl der W?rfe f?r Statistik

        int commonCount = 0;
        int uncommonCount = 0;
        int rareCount = 0;
        int mythicCount = 0;
        int legendaryCount = 0;

        for (int i = 0; i < testRuns; i++)
        {
            ItemRarity rarity = rarityScalingData.GetRarityForLevel(playerLevel);
            switch (rarity)
            {
                case ItemRarity.Common:
                    commonCount++;
                    break;
                case ItemRarity.Uncommon:
                    uncommonCount++;
                    break;
                case ItemRarity.Rare:
                    rareCount++;
                    break;
                case ItemRarity.Mythic:
                    mythicCount++;
                    break;
                case ItemRarity.Legendary:
                    legendaryCount++;
                    break;
            }
        }

        Debug.Log($"Rarity distribution at level {playerLevel} ({testRuns} runs):");
        Debug.Log($"Common: {commonCount} ({(commonCount / (float) testRuns) * 100f:F2}%)");
        Debug.Log($"Uncommon: {uncommonCount} ({(uncommonCount / (float) testRuns) * 100f:F2}%)");
        Debug.Log($"Rare: {rareCount} ({(rareCount / (float) testRuns) * 100f:F2}%)");
        Debug.Log($"Mythic: {mythicCount} ({(mythicCount / (float) testRuns) * 100f:F2}%)");
        Debug.Log($"Legendary: {legendaryCount} ({(legendaryCount / (float) testRuns) * 100f:F2}%)");
    }

}
