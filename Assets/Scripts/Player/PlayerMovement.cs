using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private RectTransform healthBarTransform;

    [SerializeField] private GameManager gameManager;
    private Vector2 movement;


    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (movement.x != 0) ChangeRotation();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * (speed * PlayerStats.instance.SpeedMultiplier * Time.fixedDeltaTime));
    }

    private void ChangeRotation()
    {
        if (gameManager.currentState != GameManager.GameState.GamePlay) return;
        if (movement.x <= 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            healthBarTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            healthBarTransform.localScale = new Vector3(1, 1, 1);
        }
    }

    public bool IsWalking()
    {
        return movement != Vector2.zero;
    }
}