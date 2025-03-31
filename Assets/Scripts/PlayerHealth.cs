using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    private float _currentHealth;
    [SerializeField] private PlayerAnimator playerAnimator;
    private CircleCollider2D _collider;
    private PlayerMovement _playerMovement;
    private Canvas _playerCanvas;

    [SerializeField] private Image healthBar;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InventoryManager inventoryManager;
    
    private void Awake()
    {
        HealthItem.OnHealthItemCollect += Heal;
        _currentHealth = PlayerStats.instance.MaxHealth;
        _collider = GetComponent<CircleCollider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCanvas = GetComponentInChildren<Canvas>();
    }

    private float _healCountdown = 0;
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
        
        if (_currentHealth <= 0)
        {
            Die();
        }
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
        if (!gameManager.IsGameOver)
        {
            Invoke("GameOver", 1.5f);
            //gameManager.GameOver();
        }
        gameObject.tag = "Untagged";
        _collider.enabled = false;
        playerAnimator.PlayDeathAnimation();
        _playerMovement.enabled = false;
        _playerCanvas.enabled = false;
        Destroy(gameObject, 2f);
        //Time.timeScale = 0;
    }

    private void GameOver()
    {
        gameManager.AssignLevelReached(levelManager.GetCurrentLevel());
        gameManager.AssignChoosenWeaponsAndPassives(inventoryManager.weaponUISlots, inventoryManager.currentWeaponIndex, inventoryManager.passiveUISlots, inventoryManager.currentPassiveIndex);
        gameManager.GameOver();
    }
}
