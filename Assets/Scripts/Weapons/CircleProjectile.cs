using UnityEngine;

public class CircleProjectile : MonoBehaviour
{
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform projVisual;
    
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float knockbackDuration = 0.2f;
    protected float _fireCountdown;
    
    [SerializeField] private GameObject effectPrefab;

    private void Start()
    {
        if (effectPrefab)
        {
            var effect = Instantiate(effectPrefab, transform.position + Vector3.up, Quaternion.identity);
            Destroy(effect, 0.6f);
        }
        projVisual.localScale = new Vector3(projVisual.localScale.x * range * 2, projVisual.localScale.y * range * 2,
            projVisual.localScale.z);
        // projVisual.localScale = new Vector3(range * 2, range * 2, 1);
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
        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, range, enemyLayers);
        foreach (var enemy in hitEnemies) enemy.GetComponent<Enemy>().TakeDamage(damage, this.transform.position, knockbackForce, knockbackDuration);
    }

    public void Setup(float fireRate, float range, float damage)
    {
        this.fireRate = fireRate;
        this.range = range;
        this.damage = damage;
    }
}