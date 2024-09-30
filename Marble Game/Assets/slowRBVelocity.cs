using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowRBVelocity : MonoBehaviour
{
    private List<Rigidbody2D> rbs = new List<Rigidbody2D>();

    [SerializeField][Range(0f,1f)] private float slowdownFactor = 0.95f;

    private void FixedUpdate()
    {
        foreach (Rigidbody2D rb in rbs)
        {
            rb.velocity *= slowdownFactor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>())
        {
            rbs.Add(other.GetComponent<Rigidbody2D>());
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>())
        {
            rbs.Remove(other.GetComponent<Rigidbody2D>());
        }
    }
}
