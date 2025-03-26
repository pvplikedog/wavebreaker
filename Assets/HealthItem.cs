using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, ICollectable
{
    public static event Action<float> OnHealthItemCollect;
    [SerializeField] private float worth;

    public void Collect()
    {
        OnHealthItemCollect?.Invoke(worth);
        Destroy(gameObject);
    }
}
