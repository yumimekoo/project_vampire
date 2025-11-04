using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOverTimeOnPlayerHit : ISkillEffect
{
    public string EffectName => "HealOverTimeOnPlayerHit";
    private float duration;
    private float hps;
    private float cooldown;
    private float lastTriggerTime;
    private Dictionary<PlayerHealth, Coroutine> activeHeals = new();

    public HealOverTimeOnPlayerHit(float duration, float hps, float cooldown)
    {
        this.duration = duration;
        this.hps = hps;
        this.cooldown = cooldown;
    }
    public void Register()
    {
        GameEvents.OnPlayerHit += OnPlayerHit;
    }

    public void Unregister()
    {
        GameEvents.OnPlayerHit -= OnPlayerHit;
    }

    public void AddStack(float extraDuration, float extraHps)
    {
        duration += extraDuration;
        hps += extraHps;
    }

    private void OnPlayerHit(PlayerHealth player)
    {
        //Debug.Log("BleedOnHitEffect triggered");
        if (Time.time < lastTriggerTime + cooldown)
            return;
        lastTriggerTime = Time.time;

        if (player == null || activeHeals.ContainsKey(player))
            return;

        MonoBehaviour runner = player;
        Coroutine healRoutine = runner.StartCoroutine(ApplyBleed(player));
        activeHeals[player] = healRoutine;
    }

    private IEnumerator ApplyBleed(PlayerHealth player)
    {
        float elapsed = 0f;
        float tickInterval = 0.5f;
        while (elapsed < duration && player != null)
        {
            player.Heal(hps * tickInterval); // Heal pro Tick
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        activeHeals.Remove(player);
    }
}
