using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    
    public float MaxHealth = 100f;
    public float RegenerationRate = 0f;
    public float SpeedMultiplier = 1f;
    public float DamageMultiplier = 1f;
    public float XPMultiplier = 1f;
    public float DamageReducer = 0f;

    private void Awake()
    {
        instance = this;
    }
}
