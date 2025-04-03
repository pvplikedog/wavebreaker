using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageRate = 3;
    [SerializeField] private float movementSpeed = 3f;
    
    public SpriteRenderer spriteRenderer;

    private Vector2 knockbackVelocity;
    private float knockbackDuration;

    [Header("Loot")] public List<LootItem> lootItems = new();

    private CircleCollider2D _collider;

    private float _damageCountdown;
    private EnemyGFX _enemyGfx;

    private bool _isPlayerInRange;
    private bool _isStatueInRange;

    private PlayerHealth _playerHealth;
    private StatueHealth _statueHealth;
    private AIPath aiPath;
    
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        aiPath = GetComponent<AIPath>();
        _enemyGfx = GetComponent<EnemyGFX>();
        _collider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        UpgradeSpeed();
    }

    private void Update()
    {
        if (knockbackDuration > 0)
        {
            this.transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        
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
        if (collision.gameObject.CompareTag("Player")) _isPlayerInRange = false;
        if (collision.gameObject.CompareTag("Statue")) _isStatueInRange = false;
    }

    public void TakeDamage(float damage)
    {
        if (audioSource)
        {
            audioSource.Play();
        }
        
        if (damage > 0)
        {
            GameManager.GenerateFloatingText(Mathf.RoundToInt(damage).ToString(), transform);
        }
        health -= damage * PlayerStats.instance.DamageMultiplier;
        _enemyGfx.PlayTakeDamgeAnimation();
        if (health <= 0) Die();
    }
    
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        TakeDamage(dmg);

        // Apply knockback
        if (knockbackDuration > 0)
        {
            Vector2 direction = (Vector2)transform.position - sourcePosition;
            direction.Normalize();
            Knockback(direction * knockbackForce, knockbackDuration);
        }
    }

    private void Die()
    {
        LevelGoal.instance.UpdateEnemiesKilled();
        
        gameObject.tag = "Untagged";
        _collider.enabled = false;
        _enemyGfx.PlayDeathAnimation();
        DropLoot();
        Destroy(gameObject, 1f);
    }

    private void DropLoot()
    {
        foreach (var lootItem in lootItems)
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
                break;
            }
    }

    private void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            var droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
        }
    }

    private void DoDamage()
    {
        if (_playerHealth && _isPlayerInRange) _playerHealth.TakeDamage(damage);

        if (_statueHealth && _isStatueInRange) _statueHealth.TakeDamage(damage);
    }

    public void DecreaseSpeed(float percent)
    {
        movementSpeed *= 1 - percent;
        UpgradeSpeed();
    }

    private void UpgradeSpeed()
    {
        aiPath.maxSpeed = math.max(movementSpeed, 1f);
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0f)
        {
            return;
        }
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}