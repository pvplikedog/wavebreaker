using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour
{
   [Header("Unity Setup Fields")]
   [SerializeField] private Transform target;
   [SerializeField] private string enemyTag = "Enemy";
   
   [Header("Weapon setup")]
   [SerializeField] private float range = 2.5f;
   
   [SerializeField] private float fireRate = 1f;
   private float fireCountdown = 0f;

   [SerializeField] private GameObject bulletPrefab;
   [SerializeField] private Transform firePoint;

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
      if (nearestEnemy && shortestDistance <= range)
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
      if (!target)
      {     
         return;
      }

      if (fireCountdown <= 0f)
      {
         Shoot();
         fireCountdown = 1f / fireRate;
      }
      
      fireCountdown -= Time.deltaTime;
   }
   
   private void Shoot()
   {
      GameObject BulletGO =  Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
      Bullet bullet = BulletGO.GetComponent<Bullet>();
      
      if (bullet)
      {
         bullet.Seek(target);
      }
   }

   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, range);
   }
}
