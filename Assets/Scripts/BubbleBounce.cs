//using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(CircleCollider2D))]
//public class BallBouncePhysics : MonoBehaviour
//{
//    private Rigidbody2D rb;
//    private CircleCollider2D circleCollider;
//    private Vector2 screenBounds;
//    private float objectRadius;

//    [SerializeField] private float initialSpeed = 10f;
//    [SerializeField] private float sizeFactor = 1f;

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        circleCollider = GetComponent<CircleCollider2D>();

//        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

//        objectRadius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

//        float speed = initialSpeed * sizeFactor;

//        rb.velocity = new Vector2(Random.Range(-2f, 2f), speed);
//    }

//    private void FixedUpdate()
//    {
//        Vector2 currentPosition = transform.position;

//        if (currentPosition.x <= -screenBounds.x + objectRadius || currentPosition.x >= screenBounds.x - objectRadius)
//        {
//            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);

//            currentPosition.x = Mathf.Clamp(currentPosition.x, -screenBounds.x + objectRadius, screenBounds.x - objectRadius);
//            transform.position = currentPosition;
//        }

//        if (currentPosition.y <= -screenBounds.y + objectRadius || currentPosition.y >= screenBounds.y - objectRadius)
//        {
//            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);

//            currentPosition.y = Mathf.Clamp(currentPosition.y, -screenBounds.y + objectRadius, screenBounds.y - objectRadius);
//            transform.position = currentPosition;
//        }
//    }
//}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BallBouncePhysics : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 screenBounds;
    private float objectRadius;

    [SerializeField]
    private float initialSpeed = 10f;

    [SerializeField]
    private float sizeFactor = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectRadius = GetComponent<CircleCollider2D>().radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

        float speed = initialSpeed * sizeFactor;
        rb.velocity = new Vector2(Random.Range(-2f, 2f), speed);
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = transform.position;

        if (currentPosition.x <= -screenBounds.x + objectRadius || currentPosition.x >= screenBounds.x - objectRadius)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            currentPosition.x = Mathf.Clamp(currentPosition.x, -screenBounds.x + objectRadius, screenBounds.x - objectRadius);
            transform.position = currentPosition;
        }

        if (currentPosition.y <= -screenBounds.y + objectRadius || currentPosition.y >= screenBounds.y - objectRadius)
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            currentPosition.y = Mathf.Clamp(currentPosition.y, -screenBounds.y + objectRadius, screenBounds.y - objectRadius);
            transform.position = currentPosition;
        }
    }
}