using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private BoxCollider2D col;
    private bool hasMoved = false;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        hasMoved = false;

        Vector3 movement = new Vector3(10f, 0, 0) * Time.deltaTime;
        Vector3 movementToDo = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        { 
            movementToDo += -movement; 
            hasMoved = true;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movementToDo += movement;
            hasMoved = true;
        }

        if (hasMoved)
        {
            Vector3 newPosition = transform.position + movementToDo;
            
            if (IsInside(newPosition))
            {
                transform.position = newPosition;
            }
        }
    }

    bool IsInside(Vector3 position)
    {
        Vector3 colOffset = col.offset;
        float colliderWidth = col.bounds.size.x;
        Vector3 scale = transform.localScale;

        Vector3 screenMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float colMinX = position.x + colOffset.x*scale.x - colliderWidth / 2;
        float colMaxX = position.x + colOffset.x*scale.x + colliderWidth / 2;

        return colMinX > screenMin.x && colMaxX < screenMax.x;
    }
}