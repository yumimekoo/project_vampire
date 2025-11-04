using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Dictionary<string, ISkillEffect> activeEffects = new();

    public void ApplyItem(ShopItemDataSO item)
    {
        foreach (var passive in item.passiveSkillEffects)
        {
            if(activeEffects.TryGetValue(passive.effectName, out ISkillEffect existing))
            {
                existing.AddStack(passive.value1, passive.value2);
                Debug.Log($"Stacked existing effect: {passive.effectName}");
            }
            else
            {
                ISkillEffect newEffect = CreateEffectFromData(passive);
                if(newEffect != null)
                {
                    newEffect.Register();
                    activeEffects.Add(passive.effectName, newEffect);
                    Debug.Log($"Registered new effect: {passive.effectName}");
                }
                else
                {
                    Debug.LogWarning($"Failed to create effect: {passive.effectName}");
                }
            }
        }
    }

    private ISkillEffect CreateEffectFromData(ShopItemDataSO.PassiveSkillEffect data)
    {
        switch (data.effectName)
        {
            case "BleedOnHit":
                            return new BleedOnHitEffect(data.value1, data.value2, data.cooldown);
            // case "AnotherEffect": etc.
            default:
                return null;
        }
    }

    public void ClearEffects()
    {
        foreach (var e in activeEffects.Values)
        {
            e.Unregister();
        }
        activeEffects.Clear();
    }
}
