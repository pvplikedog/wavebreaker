using UnityEngine;

public class GunWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    
    [SerializeField] private Transform target;
    
    [SerializeField] private float range = 3f;

    private void Update()
    {
        UpdateTarget();
        if (!target)
        {     
            return;
        }

        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / fireRate;
        }
      
        _fireCountdown -= Time.deltaTime;
    }

    private void UpdateTarget()
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
    
    private void Shoot()
    {
        GameObject BulletGO =  Instantiate(bulletPrefab, transform.position, transform.rotation);
        GunBullet bullet = BulletGO.GetComponent<GunBullet>();
      
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
    
    public override void Upgrade()
    {
        switch (_curLvl)
        {
            case 1:
                fireRate *= 2;
                _curLvl++;
                break;
            case 2:
                range *= 1.5f;
                _curLvl++;
                break;
            case 3:
                fireRate *= 1.5f;
                _curLvl++;
                break;
            case 4:
                fireRate *= 1.5f;
                range *= 1.5f;
                _curLvl++;
                break;
        }
    }
}
