using UnityEngine;

public class HardGunWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float timeLast = 3f;
    [SerializeField] private float bulletFireRate = 3f;
    [SerializeField] private float bulletRadius = 0.2f;
    [SerializeField] private float bulletSpeed = 1f;
    
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
        target = nearestEnemy ? nearestEnemy.transform : null;
    }
    
    private void Shoot()
    {
        GameObject BulletGO =  Instantiate(bulletPrefab, transform.position, transform.rotation);
        BulletGO.GetComponent<CircleProjectile>().Setup(bulletFireRate, bulletRadius, damage);
        BulletGO.GetComponent<Rigidbody2D>().AddForce((target.position - transform.position) * bulletSpeed, ForceMode2D.Impulse);
        Destroy(BulletGO, timeLast);
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
                bulletRadius *= 1.5f;
                bulletFireRate *= 1.5f;
                _curLvl++;
                break;
            case 3:
                damage *= 1.5f;
                _curLvl++;
                break;
            case 4:
                bulletRadius *= 1.5f;
                damage *= 1.5f;
                _curLvl++;
                break;
        }
    }
}
