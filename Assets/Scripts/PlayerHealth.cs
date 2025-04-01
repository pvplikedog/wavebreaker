using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerAnimator playerAnimator;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InventoryManager inventoryManager;
    private CircleCollider2D _collider;
    private float _currentHealth;

    private float _healCountdown;
    private Canvas _playerCanvas;
    private PlayerMovement _playerMovement;
    
    [SerializeField] private ParticleSystem damageEffect;

    private void Awake()
    {
        HealthItem.OnHealthItemCollect += Heal;
        _collider = GetComponent<CircleCollider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCanvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        _currentHealth = PlayerStats.instance.MaxHealth;
    }

    private void Update()
    {
        // Heals each second.
        if (_healCountdown <= 0f)
        {
            Heal(PlayerStats.instance.RegenerationRate);
            _healCountdown = 1f;
        }

        _healCountdown -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage * (1 - PlayerStats.instance.DamageReducer);

        UpdateHealthBar();

        if (damageEffect)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        if (_currentHealth <= 0) Die();
    }

    public void Heal(float heal)
    {
        _currentHealth = math.min(_currentHealth + heal, PlayerStats.instance.MaxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = _currentHealth / PlayerStats.instance.MaxHealth;
    }

    private void Die()
    {
        if (!gameManager.IsGameOver) GameOver();
        //gameManager.GameOver();
        gameObject.tag = "Untagged";
        _collider.enabled = false;
        playerAnimator.PlayDeathAnimation();
        _playerCanvas.enabled = false;
        Destroy(gameObject, 2f);
        //Time.timeScale = 0;
    }

    public void GameOver()
    {
        _playerMovement.enabled = false;
        gameManager.AssignLevelReached(levelManager.GetCurrentLevel());
        gameManager.AssignChoosenWeaponsAndPassives(inventoryManager.weaponUISlots, inventoryManager.currentWeaponIndex,
            inventoryManager.passiveUISlots, inventoryManager.currentPassiveIndex);
        Invoke("GameOverWithDelay", 1.5f);
    }

    private void GameOverWithDelay()
    {
        gameManager.GameOver();
    }
}