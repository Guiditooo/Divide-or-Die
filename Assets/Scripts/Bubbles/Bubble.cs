using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct BubbleCollision
{
    public Bubble From;
    public Bubble To;
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bubble : MonoBehaviour
{
    public Action<BubbleCollision> OnCollisionWithPair;
    public Action<Bubble> OnCollisionWithBullet;

    private Rigidbody2D rb;
    private BubbleData data;
    public BubbleData Data { get { return data;} }

    private Vector2 screenBounds;
    private float objectRadius;

    private bool wokeUp;

    private int hp;
    public int HP { get { return hp; } }

    public void Initialize(BubbleData data, Rigidbody2D rb, int level)
    {
        this.data = data;
        this.rb = rb;
        hp = (int)(data.hpFactor * (level + data.baseHP));

        wokeUp = false;

        InitRotator(data.minRotation, data.maxRotation);
        InitBouncer(data.initialSpeed, data.sizeFactor);
        WakeUp(data.wakeUpTime);
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
        //Update UI
        //Do Sound
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.collider.tag != tag)
        {
            OnCollisionWithBullet?.Invoke(this);
            return;
        }

        BubbleCollision col = new BubbleCollision()
        {
            From = this,
            To = collision.collider.gameObject.GetComponent<Bubble>()
        };

        OnCollisionWithPair?.Invoke(col);
    }

    #region BubbleBounce
    private void InitBouncer(float initialSpeed, float sizeFactor)
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectRadius = GetComponent<CircleCollider2D>().radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

        float speed = initialSpeed * sizeFactor;
        rb.velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f), speed);
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