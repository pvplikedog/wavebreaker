using System;
using UnityEngine;

[Serializable]
public class LootItem
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}