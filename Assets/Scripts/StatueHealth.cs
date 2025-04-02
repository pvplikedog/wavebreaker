using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class StatueHealth : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D collider;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerHealth playerHealth;
    
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private ParticleSystem deathEffect;
    
    [SerializeField] private SpriteRenderer statueVisual;

    private float _currentHealth;

    private float _healCountdown;

    private void Start()
    {
        _currentHealth = PlayerStats.instance.StatueMaxHealth;
    }

    private void Update()
    {
        if (_healCountdown <= 0f)
        {
            Heal(PlayerStats.instance.StatueRegenerationRate);
            _healCountdown = 1f;
        }

        _healCountdown -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking damage");
        _currentHealth -= damage * (1 - PlayerStats.instance.StatueDamageReducer);

        UpdateHealthBar();
        
        if (damageEffect)
        {
            Instantiate(damageEffect, transform.position + Vector3.up, Quaternion.identity);
        }

        if (_currentHealth <= 0) Die();
    }

    public void Heal(float heal)
    {
        _currentHealth = math.min(_currentHealth + heal, PlayerStats.instance.StatueMaxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = _currentHealth / PlayerStats.instance.StatueMaxHealth;
    }

    private void Die()
    {
        if (!gameManager.IsGameOver) playerHealth.GameOver();
        gameObject.tag = "Untagged";
        collider.enabled = false;
        
        statueVisual.enabled = false;

        if (damageEffect)
        {
            Instantiate(deathEffect, transform.position + Vector3.up, Quaternion.identity);
        }
        
        
        // play animation here.
        //playerAnimator.PlayDeathAnimation();

        //_playerCanvas.enabled = false;
        Destroy(gameObject, 2f);
    }
}