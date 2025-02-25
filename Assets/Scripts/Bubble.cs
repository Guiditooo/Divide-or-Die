using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bubble : MonoBehaviour
{
    [SerializeField] float minRotation = -100.0f;
    [SerializeField] float maxRotation = 100.0f;
    
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float randomSpeed = Random.Range(minRotation, maxRotation);
        rb.angularVelocity = randomSpeed;
    }
}