using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private EnemyGFX _enemyGfx;
    private CircleCollider2D _collider;

    private void Awake()
    {
        _enemyGfx = GetComponent<EnemyGFX>();
        _collider = GetComponent<CircleCollider2D>();
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        _enemyGfx.PlayTakeDamgeAnimation();
        if (health <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        gameObject.tag = "Untagged";
        _collider.enabled = false;
        _enemyGfx.PlayDeathAnimation();
        Destroy(gameObject, 1f);
    }
}
