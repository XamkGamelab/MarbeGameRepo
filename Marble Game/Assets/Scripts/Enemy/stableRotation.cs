using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class stableRotation : MonoBehaviour
{
    [SerializeField][Range(1.0f, 10.0f)] private float rotationSpeed = 3f;
    [SerializeField] private float rngRotation;
    private Rigidbody2D rb;
    [SerializeField] private bool randomizeDirection;
    [SerializeField][Range(0, 1)] private int direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (randomizeDirection)
        {
            direction = Random.Range(0, 2);
        }

        rotationSpeed += Random.Range(-rngRotation, rngRotation);
    }

    private void FixedUpdate()
    {
        if (direction == 0)
        {
            rb.MoveRotation(rb.rotation + (rotationSpeed * 10) * Time.fixedDeltaTime);
        }
        else
        {
            rb.MoveRotation(rb.rotation - (rotationSpeed * 10) * Time.fixedDeltaTime);
        }
    }
}
