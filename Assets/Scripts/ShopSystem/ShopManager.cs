using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItemDataSO> allItems;
    [SerializeField] private UIDocument shopUI;
    [SerializeField] private int rerollCost = 50;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private RarityScalingDataSO rarityScalingData;

    private List<ShopItemDataSO> currentItems = new List<ShopItemDataSO>();
    private VisualElement root,
        itemContainer;
    private Button rerollButton;
    private Label moneyLabel;
    private Dictionary<ShopItemDataSO, VisualElement> itemElements = new();

    private void Awake()
    {
        root = shopUI.rootVisualElement;
        itemContainer = root.Q<VisualElement>("itemContainer");
        rerollButton = root.Q<Button>("rerollButton");
        moneyLabel = root.Q<Label>("moneyLabel");

        rerollButton.clicked += RerollShop;
    }

    private void Start()
    {
        RollItems();
    }

    public void RollItems()
    {
        itemContainer.Clear();
        currentItems.Clear();

        var possibleItems = GetItemsForCurrentLevel();

        for (int i = 0; i < 3; i++)
        {
            var randomItem = possibleItems[UnityEngine.Random.Range(0, possibleItems.Count)];
            currentItems.Add(randomItem);
            CreateItemUI(randomItem);
        }
    }

    private void CreateItemUI(ShopItemDataSO itemData)
    {
        var itemElement = new VisualElement();
        itemElement.AddToClassList("shop-item");

        var nameLabel = new Label(itemData.itemName);
        nameLabel.AddToClassList("shop-name");
        var descLabel = new Label(itemData.description);
        descLabel.AddToClassList("shop-desc");
        var priceLabel = new Label($"{itemData.basePrice} $");
        priceLabel.AddToClassList("shop-price");
        var buyButton = new Button(() => BuyItem(itemData)) { text = "Buy"};
        buyButton.AddToClassList("shop-buy");
        itemElement.Add(nameLabel);
        itemElement.Add(descLabel);
        itemElement.Add(priceLabel);
        itemElement.Add(buyButton);

        itemContainer.Add(itemElement);
        itemElements[itemData] = itemElement;
    }

    private void BuyItem(ShopItemDataSO item)
    {
        if (levelManager.TrySpendMoney(item.basePrice))
        {
            ApplyItemEffect(item);
            if (itemElements.TryGetValue(item, out var element))
            {
                element.AddToClassList("shop-item-disabled");
                var button = element.Q<Button>();
                button.SetEnabled(false);
            }
        }
        else
        {
            // play sound and visual here
            Debug.Log("Not Enough Money");
        }
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        moneyLabel.text = $"Money: {levelManager.GetMoney()}";
    }

    private void ApplyItemEffect(ShopItemDataSO item)
    {
        // TODO: kommt noch mit switch case? 
        Debug.Log($"Bought {item.itemName}");
    }

    private void RerollShop()
    {
        if (levelManager.TrySpendMoney(rerollCost))
        {
            RollItems();
        }
        else
        {
            Debug.Log("Not enough Money");
        }
    }

    private List<ShopItemDataSO> GetItemsForCurrentLevel()
    {
        ItemRarity rarity = rarityScalingData.GetRarityForLevel(levelManager.GetLevel());
        var allItems = Resources.LoadAll<ShopItemDataSO>("ShopItems");
        return allItems.Where(i => i.rarity == rarity).ToList();
    }

    [ContextMenu("Test Rarity Distribution")]
    private void TestRarityDistribution()
    {
        int playerLevel = 25; // Oder beliebiger Test-Level
        int testRuns = 10000; // Anzahl der Würfe für Statistik

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
