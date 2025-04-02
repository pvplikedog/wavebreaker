using System;
using UnityEngine;

public class MassDestructWeapon : Weapon
{
    [SerializeField] private float damage = 10f;
    
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private GameObject effectPrefab;

    private void Update()
    {
        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / fireRate;
        }

        _fireCountdown -= Time.deltaTime * PlayerStats.instance.StatueFireRateMultiplier;
    }

    private void Shoot()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Performance issue, probably will need to rework.
        if (enemies.Length <= 0) return;
        
        foreach (var enemy in enemies) enemy.GetComponent<Enemy>().TakeDamage(damage);
        var effect = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);
        Destroy(effect, 0.5f);
    }

    public override void Upgrade()
    {
        switch (_curLvl)
        {
            case 1:
                damage *= 1.5f;
                _curLvl++;
                break;
            case 2:
                fireRate *= 1.5f;
                _curLvl++;
                break;
            case 3:
                damage *= 1.5f;
                _curLvl++;
                break;
            case 4:
                fireRate *= 1.5f;
                damage *= 1.5f;
                _curLvl++;
                break;
        }
    }
}