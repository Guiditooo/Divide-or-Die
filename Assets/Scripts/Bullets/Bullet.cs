using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10;

    public void Initialize(BulletData data)
    {
        speed = data.speed;
    }

    private void Update()
    {
        Vector3 movement = gameObject.transform.up * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
