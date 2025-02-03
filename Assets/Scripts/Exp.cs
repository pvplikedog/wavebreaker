using System;
using Unity.VisualScripting;
using UnityEngine;

public class Exp : MonoBehaviour, ICollectable
{
    public static event Action<float> OnExpCollect;
    [SerializeField] private float worth;
    
    public void Collect()
    {
        OnExpCollect?.Invoke(worth);
        Destroy(gameObject);
    }
}
