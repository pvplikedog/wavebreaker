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
    
    public float StatueMaxHealth = 100f;
    public float StatueRegenerationRate = 0.5f;
    public float StatueDamageReducer = 0f;
    public float StatueFireRateMultiplier = 1f;

    private void Awake()
    {
        instance = this;
    }
}
