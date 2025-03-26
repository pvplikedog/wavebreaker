using UnityEngine;

public class MeleeCircleWeapon : Weapon
{
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform circleVisual;

    private void Start()
    {
        circleVisual.localScale = new Vector3(range * 2, range * 2, 1);
    }
    
    private void Update()
    {
        if (_fireCountdown <= 0f)
        {
            DoDamage();
            _fireCountdown = 1f / fireRate;
        }
      
        _fireCountdown -= Time.deltaTime;
    }

    private void DoDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, range, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
    
    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }
}
