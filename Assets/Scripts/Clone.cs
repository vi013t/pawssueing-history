using UnityEngine;
using System.Collections.Generic;

public class Clone : MonoBehaviour, Damageable
{
    public Queue<Vector2> movements = new();
    private Rigidbody2D rigidBody;

    public int Health { get; set; }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Health = 1;
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

    public void Damage(int damage)
    {
        Health-=damage;
        if(Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}