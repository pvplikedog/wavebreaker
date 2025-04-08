using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private AudioSource pickUpSound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var collectable = other.GetComponent<ICollectable>();
        if (collectable != null)
        {
            pickUpSound.Play();
            collectable.Collect();
        }
    }
}