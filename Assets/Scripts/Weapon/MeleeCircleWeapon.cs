using UnityEngine;

public class MeleeCircleWeapon : Weapon
{
    [SerializeField] private float range = 2.5f;
    [SerializeField] private float damage = 10f;

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
        
    }
    
    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }
}
