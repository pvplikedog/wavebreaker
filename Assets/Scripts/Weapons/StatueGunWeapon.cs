using UnityEngine;

public class StatueGunWeapon : Weapon
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform target;

    [SerializeField] private float range = 3f;
    
    private AudioSource _audio;
    
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
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

        _fireCountdown -= Time.deltaTime * PlayerStats.instance.StatueFireRateMultiplier;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
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
        if (_audio && !_audio.isPlaying)
        {
            _audio.Play();
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