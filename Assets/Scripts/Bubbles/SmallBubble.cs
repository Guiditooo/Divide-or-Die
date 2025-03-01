using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmallBubble : Bubble, IMergeable
{
    public Action<BubbleBubbleCollision> OnCollisionWithPair;
    public void Merge(IMergeable other)
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf)
            return;
        Debug.Log("Me Choque");

        BubbleBubbleCollision col = new BubbleBubbleCollision()
        {
            From = this,
            To = collision.collider.gameObject.GetComponent<Bubble>()
        };

        OnCollisionWithPair?.Invoke(col);

        //collision.collider.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }
}
