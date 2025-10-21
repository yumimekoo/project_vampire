using UnityEngine;

public class TESTING : MonoBehaviour
{

    [SerializeField] PlayerStatsManager playerStatsManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(playerStatsManager.GetStat(StatType.MoveSpeed));
        Debug.Log(playerStatsManager.GetStat(StatType.DashRegenerationRate));
    }
}
