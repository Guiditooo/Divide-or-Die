using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    private void Update()
    {
        Vector3 movement = gameObject.transform.up * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}
