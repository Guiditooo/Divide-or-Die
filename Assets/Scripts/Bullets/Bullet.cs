using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action<Bullet> OnRecycle;

    private BoxCollider2D col;
    private float speed;

    private void OnEnable()
    {
        col ??= GetComponent<BoxCollider2D>();
        col.enabled = true;
    }

    public void Initialize(BulletData data)
    {
        speed = data.speed;
    }

    private void Update()
    {
        Vector3 movement = gameObject.transform.up * speed * Time.deltaTime;
        transform.Translate(movement);
        Physics2D.SyncTransforms();
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.layer != 11)//!BulletRecycler
            return;

        col.enabled = false;
        RecycleIn(0.1f);
    }

    private void RecycleIn(float wakeUpTime)
    {
        StartCoroutine(WaitFor(wakeUpTime));
    }
    IEnumerator WaitFor(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        OnRecycle?.Invoke(this);
    }
}
