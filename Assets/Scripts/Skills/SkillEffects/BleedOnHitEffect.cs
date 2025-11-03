using UnityEngine;

public class BleedOnHitEffect : ISkillEffect
{
    private float duration;
    private float dps;

    public BleedOnHitEffect(float duration, float dps)
    {
        this.duration = duration;
        this.dps = dps;
    }
    public void Register()
    {
        GameEvents.OnBulletHit += OnBulletHit;
    }

    public void Unregister()
    {
        GameEvents.OnBulletHit -= OnBulletHit;
    }

    private void OnBulletHit(Bullet bullet, EnemyBase enemy)
    {
        Debug.Log("BleedOnHitEffect triggered");
    }
}
