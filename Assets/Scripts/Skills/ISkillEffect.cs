using UnityEngine;

public interface ISkillEffect
{
    void Register();
    void Unregister();
    void AddStack(float value1, float value2);

    string EffectName { get; }
}
