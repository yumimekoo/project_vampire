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

    private List<ShopItemDataSO> currentItems = new List<ShopItemDataSO>();
    private VisualElement root,
        itemContainer;
    private Button rerollButton;
    private Label moneyLabel;

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

    private void RollItems()
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
        var descLabel = new Label(itemData.description);
        var priceLabel = new Label($"{itemData.basePrice} $");
        var buyButton = new Button(() => BuyItem(itemData)) { text = "Buy"};

        itemElement.Add(nameLabel);
        itemElement.Add(descLabel);
        itemElement.Add(priceLabel);
        itemElement.Add(buyButton);

        itemContainer.Add(itemElement);

    }

    private void BuyItem(ShopItemDataSO item)
    {
        if (levelManager.TrySpendMoney(item.basePrice))
        {
            ApplyItemEffect(item);
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
        // hier die rarity berechnung mit level etc,. 
        return allItems.Where(i => i.rarity == ItemRarity.Common).ToList();
    }
}
