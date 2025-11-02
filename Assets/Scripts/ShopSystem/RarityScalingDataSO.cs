using UnityEngine;

[CreateAssetMenu(fileName = "RarityScalingData", menuName = "Scriptable Objects/RarityScalingData")]
public class RarityScalingDataSO : ScriptableObject
{
    public AnimationCurve commonCurve;
    public AnimationCurve uncommonCurve;
    public AnimationCurve rareCurve;
    public AnimationCurve epicCurve;
    public AnimationCurve legendaryCurve;

    public int maxLevel = 100;

    public ItemRarity GetRarityForLevel(int level)
    {
        float t = Mathf.Clamp01((float) level / maxLevel);

        float roll = Random.value;
        float c = commonCurve.Evaluate(t);
        float u = uncommonCurve.Evaluate(t);
        float r = rareCurve.Evaluate(t);
        float e = epicCurve.Evaluate(t);
        float l = legendaryCurve.Evaluate(t);

        float total = c + r + u + e + l;
        roll *= total;

        if (roll <= c)
            return ItemRarity.Common;
        if (roll <= c + u)
            return ItemRarity.Uncommon;
        if (roll <= c + r + u)
            return ItemRarity.Rare;
        if (roll <= c + r + u + e)
            return ItemRarity.Mythic;
        return ItemRarity.Legendary;
    }
}
