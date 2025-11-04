using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedOnHitEffect : ISkillEffect
{
    public string EffectName => "BleedOnHit";
    private float duration;
    private float dps;
    private float cooldown;
    private float lastTriggerTime;
    private Dictionary<EnemyBase, Coroutine> activeBleeds = new();

    public BleedOnHitEffect(float duration, float dps, float cooldown)
    {
        this.duration = duration;
        this.dps = dps;
        this.cooldown = cooldown;
    }
    public void Register()
    {
        GameEvents.OnBulletHit += OnBulletHit;
    }

    public void Unregister()
    {
        GameEvents.OnBulletHit -= OnBulletHit;
    }

    public void AddStack(float extraDuration, float extraDps)
    {
        duration += extraDuration;
        dps += extraDps;
    }

    private void OnBulletHit(Bullet bullet, EnemyBase enemy)
    {
        //Debug.Log("BleedOnHitEffect triggered");
        if (Time.time < lastTriggerTime + cooldown)
            return;
        lastTriggerTime = Time.time;

        if (enemy == null || activeBleeds.ContainsKey(enemy))
            return;

        MonoBehaviour runner = enemy;
        Coroutine bleedRoutine = runner.StartCoroutine(ApplyBleed(enemy));
        activeBleeds[enemy] = bleedRoutine;
    }

    private IEnumerator ApplyBleed (EnemyBase enemy)
    {
        float elapsed = 0f;
        float tickInterval = 0.2f;
        while (elapsed < duration && enemy != null)
        {
            enemy.TakeDamage(dps * tickInterval);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        activeBleeds.Remove(enemy);
    }
}
