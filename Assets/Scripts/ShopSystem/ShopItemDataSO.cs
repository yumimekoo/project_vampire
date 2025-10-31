using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity { Common, Uncommon, Rare, Mythic, Legendary, Goofy }
public enum ItemType { StatUpgrade, ActiveSkill, PassiveSkill }

[CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "Scriptable Objects/ShopItemDataSO")]
public class ShopItemDataSO : ScriptableObject
{
    [System.Serializable]
    public struct StatEffect
    {
        public StatType statType;
        public float value;
    }

    public string itemName;
    public string description;
    public ItemType type;
    public ItemRarity rarity;
    public int basePrice;
    public Sprite icon;

    [Header("Positive Effects")]
    public List<StatEffect> positiveEffects = new List<StatEffect>();

    [Header("Negative Effects")]
    public List<StatEffect> negativeEffects = new List<StatEffect>();
}

