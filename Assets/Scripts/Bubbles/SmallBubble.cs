using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmallBubble : Bubble, IMergeable
{
    public Action<BubbleCollision> OnCollisionWithPair;
    public void Merge(IMergeable other)
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf)
            return;
        Debug.Log("Me Choque");

        BubbleCollision col = new BubbleCollision()
        {
            From = this,
            To = collision.collider.gameObject.GetComponent<Bubble>()
        };

        OnCollisionWithPair?.Invoke(col);

        //collision.collider.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }
}
