using UnityEngine;

public class FreezingWeapon : Weapon
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float freezePower = 0.4f;
    [SerializeField] private int targetAmount = 1;

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
        var effect = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);
        Destroy(effect, 0.5f);
        for (int i = 0; i < targetAmount; i++)
        {
            var randomEnemy = enemies[UnityEngine.Random.Range(0, enemies.Length)];
            if (randomEnemy)
            {
                var enemy = randomEnemy.GetComponent<Enemy>();
                if (!enemy) return;
                enemy.DecreaseSpeed(freezePower);
                enemy.TakeDamage(damage);
                enemy.spriteRenderer.color = new Color(79/255f, 154/255f, 217/255f);
            }
        }
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
                targetAmount = 2;
                _curLvl++;
                break;
            case 3:
                freezePower *= 1.5f;
                _curLvl++;
                break;
            case 4:
                targetAmount = 3;
                _curLvl++;
                break;
        }
    }
}
