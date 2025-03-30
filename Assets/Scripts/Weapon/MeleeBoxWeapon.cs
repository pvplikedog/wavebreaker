using UnityEngine;

public class MeleeBoxWeapon : Weapon
{
    [SerializeField] private float damage = 10f;
    
    [Header("Box settings")]
    [SerializeField] float boxWidth;
    [SerializeField] float boxHeight;
    [SerializeField] private Transform boxMidPoint;
    [SerializeField] private Transform boxVisual;
    
    [Space]
    [SerializeField] private LayerMask enemyLayers;
    
    
    
    void Start()
    {
        boxVisual.position = boxMidPoint.position;
        boxVisual.localScale = new Vector3(boxWidth, boxHeight, 1);
    }
    
    void Update()
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
        Vector2 topLeft = new Vector2(boxMidPoint.position.x - boxWidth / 2, boxMidPoint.position.y + boxHeight / 2);
        Vector2 bottomRight = new Vector2(boxMidPoint.position.x + boxWidth / 2, boxMidPoint.position.y - boxHeight / 2);
        Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(topLeft, bottomRight, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    public override void Upgrade()
    {
        switch (_curLvl)
        {
            case 1:
                damage *= 1.5f;
                fireRate *= 1.5f;
                _curLvl++;
                break;
            case 2:
                boxWidth *= 2f;
                boxHeight *= 2f;
                _curLvl++;
                break;
            case 3:
                damage *= 1.5f;
                _curLvl++;
                break;
            case 4:
                fireRate *= 1.5f;
                _curLvl++;
                break;
        }
    }
}
