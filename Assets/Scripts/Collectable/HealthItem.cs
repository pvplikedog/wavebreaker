using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, ICollectable
{
    [SerializeField] private float worth;
    
    
    private void Awake()
    {
    }

    public void Collect()
    {
        OnHealthItemCollect?.Invoke(worth);
        Destroy(gameObject);
    }

    public static event Action<float> OnHealthItemCollect;
}