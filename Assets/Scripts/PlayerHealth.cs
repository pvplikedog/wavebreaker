using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth = 100f;
    [SerializeField] private PlayerAnimator playerAnimator;
    private CircleCollider2D _collider;
    private PlayerMovement _playerMovement;
    private Canvas _playerCanvas;

    [SerializeField] private Image healthBar;
    
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCanvas = GetComponentInChildren<Canvas>();
    }
    
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        
        healthBar.fillAmount = _currentHealth / maxHealth;
        
        if (_currentHealth <= 0)
        {
            Die();
        }
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
