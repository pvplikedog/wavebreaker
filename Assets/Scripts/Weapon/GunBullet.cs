using UnityEngine;

public class GunBullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float damage = 5f;
    private Transform target;

    private void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }

        var dir = target.position - transform.position;
        var distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void Seek(Transform target)
    {
        this.target = target;
    }

    private void HitTarget()
    {
        target.GetComponent<Enemy>().TakeDamage(damage);
        Destroy(gameObject);
    }
}