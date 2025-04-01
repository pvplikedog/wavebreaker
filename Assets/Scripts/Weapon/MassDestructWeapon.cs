using System;
using UnityEngine;

public class MassDestructWeapon : Weapon
{
    [SerializeField] private float damage = 10f;

    private void Update()
    {
        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / fireRate;
        }

        _fireCountdown -= Time.deltaTime;
    }

    private void Shoot()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag); // Performance issue, probably will need to rework.
        foreach (var enemy in enemies) enemy.GetComponent<Enemy>().TakeDamage(damage);
    }

    public override void Upgrade()
    {
        throw new NotImplementedException();
    }
}