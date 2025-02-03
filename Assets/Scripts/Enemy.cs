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

    private PlayerHealth _playerHealth;
    
    [Header("Loot")]
    public List<LootItem> lootItems = new List<LootItem>();

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
