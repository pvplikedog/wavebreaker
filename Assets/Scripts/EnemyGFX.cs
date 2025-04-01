using Pathfinding;
using UnityEngine;

public class EnemyGFX : MonoBehaviour
{
    private const string IS_HIT = "Hit";
    private const string IS_DEAD = "IsDead";

    [SerializeField] private Animator animator;

    private AIPath aiPath;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (aiPath.desiredVelocity.x <= -0.01f) transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public void PlayTakeDamgeAnimation()
    {
        animator.SetBool(IS_HIT, true);
    }

    public void PlayDeathAnimation()
    {
        aiPath.canMove = false;
        animator.SetTrigger(IS_DEAD);
    }
}