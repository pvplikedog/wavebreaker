using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRate = 10f;
    private float _damageCountdown = 0f;
    private EnemyGFX _enemyGfx;
    private CircleCollider2D _collider;
    
    private bool _isPlayerInRange;

    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _enemyGfx = GetComponent<EnemyGFX>();
        _collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (_isPlayerInRange)
        {
            if (_damageCountdown < 0f)
            {
                DamagePlayer();
                _damageCountdown = 1f / damageRate;
            }

            _damageCountdown -= Time.deltaTime;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            _playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            //collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
    }

    private void DamagePlayer()
    {
        Debug.Log("Damage");
        if (_playerHealth)
        {
            _playerHealth.TakeDamage(damage);
        }
    }
}
