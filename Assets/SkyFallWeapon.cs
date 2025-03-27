using UnityEngine;
using UnityEngine.Serialization;

public class SkyFallWeapon : Weapon
{
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private float damage;
    [SerializeField] private float timeLast;
    [SerializeField] private float attackRange;
    [SerializeField] private int circlesAmount;
    [SerializeField] private float circleFireRate;
    [SerializeField] private float circleRadius;
    [SerializeField] private Transform attackPoint;
    
    void Update()
    {
        if (_fireCountdown <= 0f)
        {
            for (int i = 0; i < circlesAmount; i++)
            {
                SpawnCircle();
            }
            _fireCountdown = 1f / fireRate;
        }
      
        _fireCountdown -= Time.deltaTime;
    }

    private void SpawnCircle()
    {
        GameObject circle  = Instantiate(circlePrefab);
        circle.GetComponent<CircleProjectile>().Setup(circleFireRate, circleRadius, damage);
        Vector2 randomInCircle = Random.insideUnitCircle * attackRange;
        circle.transform.position = attackPoint.position + new Vector3(randomInCircle.x, randomInCircle.y, 0);
        Destroy(circle, timeLast);
    }

    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }
}
