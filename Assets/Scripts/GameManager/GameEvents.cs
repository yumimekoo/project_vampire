using UnityEngine;

public static class GameEvents
{
    public static System.Action<Bullet> OnBulletFired;
    public static System.Action<Bullet, EnemyBase> OnBulletHit;
    public static System.Action OnDash;
    public static System.Action<PlayerHealth>OnPlayerHit;
}
