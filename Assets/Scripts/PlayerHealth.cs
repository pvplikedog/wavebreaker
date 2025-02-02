using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private PlayerAnimator playerAnimator;
    private CircleCollider2D _collider;
    private PlayerMovement _playerMovement;
    
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
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
        Destroy(gameObject, 1f);
        //Time.timeScale = 0;
    }
}
