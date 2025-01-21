using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    
    private AIPath aiPath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
