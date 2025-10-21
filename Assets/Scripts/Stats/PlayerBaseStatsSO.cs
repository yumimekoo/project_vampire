using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "PlayerBaseStatsSO", menuName = "Scriptable Objects/BaseStats")]
public class PlayerBaseStatsSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float dashDistance;
    public float dashCD;
    public float dashes;
    public float dashRegenerationRate;

    [Header("Combat")]
    public float maxHealth;
    public float attackDamage;
    public float attackSpeed;

    [Header("Other")]
    public float bulletSpeed;
    public float bulletDistance;
    public float bulletSpread;

}
