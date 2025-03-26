using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;
    [SerializeField] private PlayerAnimator playerAnimator;
    private CircleCollider2D _collider;
    private PlayerMovement _playerMovement;
    private Canvas _playerCanvas;

    [SerializeField] private Image healthBar;
    
    private void Awake()
    {
        HealthItem.OnHealthItemCollect += Heal;
        _currentHealth = maxHealth;
        _collider = GetComponent<CircleCollider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCanvas = GetComponentInChildren<Canvas>();
    }
    
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        
        UpdateHealthBar();
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float heal)
    {
        _currentHealth = math.min(_currentHealth + heal, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = _currentHealth / maxHealth;
    }
    
    private void Die()
    {
        gameObject.tag = "Untagged";
        _collider.enabled = false;
        playerAnimator.PlayDeathAnimation();
        _playerMovement.enabled = false;
        _playerCanvas.enabled = false;
        Destroy(gameObject, 2f);
        //Time.timeScale = 0;
    }
}
