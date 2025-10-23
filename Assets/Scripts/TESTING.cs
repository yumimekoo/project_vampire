using System.Collections;
using UnityEngine;

public class TESTING : MonoBehaviour
{

    [SerializeField] PlayerStatsManager playerStatsManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(playerStatsManager.GetStat(StatType.MoveSpeed));
        Debug.Log(playerStatsManager.GetStat(StatType.DashRegenerationRate));
        StartCoroutine(SetDashCooldownAfterDelay());
    }

    private IEnumerator SetDashCooldownAfterDelay()
    {
        yield return new WaitForSeconds(10f); // 10 Sekunden warten
        playerStatsManager.SetStat(StatType.AttackSpeed, 15f); // Wert setzen
        ///Debug.Log("DashCooldown auf 6 gesetzt!");
    }
}
