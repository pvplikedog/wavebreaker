using UnityEngine;

public class CircleProjectile : MonoBehaviour
{
    [SerializeField] protected float fireRate = 1f;
    protected float _fireCountdown = 0f;
    [SerializeField] private float range = 1f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform projVisual;
    
    private void Start()
    {
        projVisual.localScale = new Vector3(range * 2, range * 2, 1);
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

    public void Setup(float fireRate, float range, float damage)
    {
        this.fireRate = fireRate;
        this.range = range;
        this.damage = damage;
    }
}
