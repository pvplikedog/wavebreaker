using System;
using UnityEngine;

public class Exp : MonoBehaviour, ICollectable
{
    [SerializeField] private float worth;

    public void Collect()
    {
        OnExpCollect?.Invoke(worth);
        Destroy(gameObject);
    }

    public static event Action<float> OnExpCollect;
}