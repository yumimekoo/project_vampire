using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private List<ISkillEffect> activeEffects = new List<ISkillEffect>();

    public void ApplyItem(ShopItemDataSO item)
    {
        foreach (var passive in item.passiveSkillEffects)
        {
            ISkillEffect effect = CreateEffectFromData(passive);
            if(effect != null)
            {
                effect.Register();
                activeEffects.Add(effect);
            }
        }
    }

    private ISkillEffect CreateEffectFromData(ShopItemDataSO.PassiveSkillEffect data)
    {
        switch (data.effectName)
        {
            case "BleedOnHit":
                            return new BleedOnHitEffect(data.value1, data.value2);
            // case "AnotherEffect": etc.
            default:
                return null;
        }
    }

    public void ClearEffects()
    {
        foreach (var e in activeEffects)
        {
            e.Unregister();

            activeEffects.Clear();
        }
    }
}
