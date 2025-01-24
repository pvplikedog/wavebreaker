using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
   [SerializeField] private Transform target;
   [SerializeField] private float range = 2.5f;
   [SerializeField] private string enemyTag = "Enemy";

   private void Start()
   {
      InvokeRepeating("UpdateTarget", 0f, 0.5f);
   }
   void UpdateTarget()
   {
      GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Performance issue, probably will need to rework.
      float shortestDistance = Mathf.Infinity;
      GameObject nearestEnemy = null;
      foreach (GameObject enemy in enemies)
      {
         float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
         if (distanceToEnemy < shortestDistance)
         {
            shortestDistance = distanceToEnemy;
            nearestEnemy = enemy;
         }
      }
      if (nearestEnemy != null && shortestDistance <= range)
      {
         target = nearestEnemy.transform;
      }
      else
      {
         target = null;
      }
   }

   private void Update()
   {
      if (target == null)
      {     
         return;
      }
   }

   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, range);
   }
}
