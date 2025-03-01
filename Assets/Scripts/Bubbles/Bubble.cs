using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
public struct BubbleBubbleCollision
{
    public Bubble From;
    public Bubble To;
}
public struct BubbleBulletCollision
{
    public Bubble bubble;
    public Bullet bullet;
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bubble : MonoBehaviour
{
    public Action<BubbleBubbleCollision> OnCollisionWithPair;
    public Action<BubbleBulletCollision> OnCollisionWithBullet;
    public Action OnCollisionWithPlayer;

    private Rigidbody2D rb;
    private CircleCollider2D col;

    private BubbleData data;
    public BubbleData Data { get { return data;} }

    private Vector2 screenBounds;
    private float objectRadius;

    private bool wokeUp;

    private int hp;
    public int HP { get { return hp; } }

    private void OnEnable()
    {
        rb ??= GetComponent<Rigidbody2D>();
        col ??= GetComponent<CircleCollider2D>();
        col.enabled = true;

        InitRotator(data.minRotation, data.maxRotation);
        InitBouncer(data.initialSpeed, data.sizeFactor);

        if (!wokeUp)
            WakeUp(data.wakeUpTime);

        Debug.Log("Enabled Bubble " + hp + " tipo: " + Data.thisType.ToString());
    }
    public void Initialize(BubbleData data, int hp, bool isNewBubble)
    {
        this.data = data;
        this.hp = hp;

        wokeUp = !isNewBubble;

        transform.localScale = new Vector3(data.scaleFactor, data.scaleFactor, data.scaleFactor);
        name = data.bubbleName;
        gameObject.layer = (int)data.thisType;

        //Debug.Log("Created " + name + " with " + hp + " hp");

    }

    private void FixedUpdate()
    {
        if (wokeUp)
        {
            Bounce();
        }
    }

    public void GetDamage(int dmgAmount)
    {
        hp -= dmgAmount;
        //Debug.Log("Yo " + name + " recibi " + dmgAmount + " y me queda " + hp + " puntos de vida");
        //Update UI
        //Do Sound
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool collisionValid = gameObject.activeSelf && collision.collider.gameObject.activeSelf;

        if (!collisionValid || collision.collider.gameObject.layer != gameObject.layer)
            return;


        BubbleBubbleCollision col = new BubbleBubbleCollision()
        {
            From = this,
            To = collision.collider.gameObject.GetComponent<Bubble>()
        };

        if(col.From.Data.mergeTo == col.To.Data.mergeTo)
            OnCollisionWithPair?.Invoke(col);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        switch (collision.gameObject.layer)
        {
            case 9://Player
                OnCollisionWithPlayer?.Invoke();
                break;
            case 10://Bullet
                BubbleBulletCollision col = new BubbleBulletCollision()
                {
                    bubble = this,
                    bullet = collision.GetComponent<Bullet>()
                };
                //Debug.Log("Tengo " + hp + " hp y me dispararon");
                OnCollisionWithBullet?.Invoke(col);
                break;
            default:
                Debug.Log("Choque con algo que no es un pj o una bala.");
                break;      
        }
        return;
    }

    #region BubbleBounce
    private void InitBouncer(float initialSpeed, float sizeFactor)
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectRadius = GetComponent<CircleCollider2D>().radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

        float speed = initialSpeed * sizeFactor;
        rb.velocity = new Vector2(UnityEngine.Random.Range(-1.5f, 1.5f), -speed);
    }
    private void Bounce()
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
    #endregion
    
    #region BubbleRotation
    void InitRotator(float minRotation, float maxRotation)
    {
        float randomSpeed = UnityEngine.Random.Range(minRotation, maxRotation);
        rb.angularVelocity = randomSpeed;
    }
    #endregion

    #region BubbleWakingUp
    private void WakeUp(float wakeUpTime)
    {
        StartCoroutine(WaitFor(wakeUpTime));
    }
    IEnumerator WaitFor(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        wokeUp = true;
    }
    #endregion
}