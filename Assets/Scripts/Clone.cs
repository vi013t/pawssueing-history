using UnityEngine;
using System.Collections.Generic;

public class Clone : MonoBehaviour
{
    public Queue<Vector2> movements = new();
    private Rigidbody2D rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (movements.Count > 0) 
        {
            rigidBody.MovePosition(rigidBody.position + movements.Dequeue());
        } else
        {
            Destroy(gameObject);
        }
    }
}