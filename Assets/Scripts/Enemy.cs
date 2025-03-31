using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRate = 3;
    private float _damageCountdown = 0f;
    private EnemyGFX _enemyGfx;
    private CircleCollider2D _collider;
    
    private bool _isPlayerInRange;
    private bool _isStatueInRange;

    private PlayerHealth _playerHealth;
    private StatueHealth _statueHealth;
    
    [Header("Loot")]
    public List<LootItem> lootItems = new List<LootItem>();

    private void Awake()
    {
        _enemyGfx = GetComponent<EnemyGFX>();
        _collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (_isPlayerInRange || _isStatueInRange)
        {
            if (_damageCountdown < 0f)
            {
                DoDamage();
                _damageCountdown = 1f / damageRate;
            }

            _damageCountdown -= Time.deltaTime;
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage * PlayerStats.instance.DamageMultiplier;
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
        DropLoot();
        Destroy(gameObject, 1f);
    }

    private void DropLoot()
    {
        foreach (LootItem lootItem in lootItems)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
                break;
            }
        }
    }
    
    private void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            _playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        }
        if (collision.gameObject.CompareTag("Statue"))
        {
            _isStatueInRange = true;
            _statueHealth = collision.gameObject.GetComponent<StatueHealth>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
        if (collision.gameObject.CompareTag("Statue"))
        {
            _isStatueInRange = false;
        }
    }

    private void DoDamage()
    {
        if (_playerHealth && _isPlayerInRange)
        {
            _playerHealth.TakeDamage(damage);
        }
        
        if (_statueHealth && _isStatueInRange)
        {
            _statueHealth.TakeDamage(damage);
        }
    }
}
