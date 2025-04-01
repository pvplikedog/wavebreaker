using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var collectable = other.GetComponent<ICollectable>();
        if (collectable != null) collectable.Collect();
    }
}