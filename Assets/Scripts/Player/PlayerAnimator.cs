using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string IS_WAKING = "IsWalking";
    private const string IS_DEAD = "IsDead";

    [SerializeField] private PlayerMovement playerMovement;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(IS_WAKING, playerMovement.IsWalking());
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(IS_DEAD);
    }
}