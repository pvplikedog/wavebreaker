using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class Weapon : MonoBehaviour
{
   [SerializeField] protected string enemyTag = "Enemy";
   
   [SerializeField] protected float fireRate = 1f;
   protected float _fireCountdown = 0f;
   protected int _curLvl = 1;

   public abstract void Upgrade();
}
