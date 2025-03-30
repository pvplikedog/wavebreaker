using Unity.VisualScripting;
using UnityEngine;

public class GunBullet : MonoBehaviour
{
    private Transform target;

    [SerializeField] private float speed = 30f;
    [SerializeField] private float damage = 5f;

    public void Seek(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        target.GetComponent<Enemy>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
