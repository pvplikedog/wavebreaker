using UnityEngine;

public class MeleeCircleWeapon : Weapon
{
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform circleVisual;
    
    private Animator animator;
    private const string IS_HIT = "Hit";

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        animator.SetTrigger(IS_HIT);

        var hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, range, enemyLayers);
        foreach (var enemy in hitEnemies) enemy.GetComponent<Enemy>().TakeDamage(damage);
    }

    public override void Upgrade()
    {
        switch (_curLvl)
        {
            case 1:
                damage *= 2f;
                fireRate *= 1.5f;
                range *= 1.5f;
                circleVisual.localScale = new Vector3(range * 2, range * 2, 1);
                _curLvl++;
                break;
            case 2:
                range *= 1.5f;
                circleVisual.localScale = new Vector3(range * 2, range * 2, 1);
                fireRate *= 1.5f;
                _curLvl++;
                break;
            case 3:
                damage *= 1.5f;
                _curLvl++;
                break;
            case 4:
                fireRate *= 2f;
                _curLvl++;
                break;
        }
    }
}