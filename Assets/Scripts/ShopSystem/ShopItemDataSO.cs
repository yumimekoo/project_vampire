using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity { Common, Uncommon, Rare, Mythic, Legendary, Goofy }
public enum ItemType { StatUpgrade, ActiveSkill, PassiveSkill }

public enum PassiveTrigger { OnBulletFired, OnBulletHit, OnEnemyKilled, OnPlayerHit, OnDash, OnLevelUp }

[CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "Scriptable Objects/ShopItemDataSO")]
public class ShopItemDataSO : ScriptableObject
{
    [System.Serializable]
    public struct StatEffect
    {
        public StatType statType;
        public float value;
    }
    [System.Serializable]
    public struct StatEffectMulti
    {
        public StatMulti statMulti;
        public float value;
    }

    [System.Serializable]
    public class PassiveSkillEffect
    {
        public string effectName;
        public PassiveTrigger trigger;
        public float value1;
        public float value2;
        public float cooldown;
    }

    public string itemName;
    public string description;
    public ItemType type;
    public ItemRarity rarity;
    public int basePrice;
    public Sprite icon;

    [Header("Positive and Negative Stat-Effects")]
    public List<StatEffect> positiveEffects = new List<StatEffect>();
    public List<StatEffectMulti> positiveEffectMultis = new List<StatEffectMulti>();
    public List<StatEffect> negativeEffects = new List<StatEffect>();
    public List<StatEffectMulti> negativeEffectMultis = new List<StatEffectMulti>();

    [Header("Passive Skill Effects")]
    public List<PassiveSkillEffect> passiveSkillEffects = new List<PassiveSkillEffect>();
}

