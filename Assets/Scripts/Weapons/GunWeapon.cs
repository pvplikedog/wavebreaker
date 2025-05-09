using UnityEngine;

public class GunWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform target;

    [SerializeField] private float range = 3f;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateTarget();
        if (!target) return;

        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / fireRate;
        }

        _fireCountdown -= Time.deltaTime;
    }

    private void UpdateTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Performance issue, probably will need to rework.
        var shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (var enemy in enemies)
        {
            var distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy && shortestDistance <= range)
            target = nearestEnemy.transform;
        else
            target = null;
    }

    private void Shoot()
    {
        if (_audioSource)
        {
            _audioSource.Play();
        }
        
        var BulletGO = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var bullet = BulletGO.GetComponent<GunBullet>();

        if (bullet) bullet.Seek(target);
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