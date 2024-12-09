using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StableRotation : MonoBehaviour
{
    [SerializeField][Range(0f, 10.0f)] private float rotationTime = 3f; // Time in seconds for a full rotation
    [SerializeField] private float rngRotation;
    private Rigidbody2D rb;
    [SerializeField] private bool randomizeDirection;
    [SerializeField][Range(0, 1)] private int direction;
    [SerializeField] private Animator anim;
    private float rotationSpeed; // Degrees per second

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    
        if (randomizeDirection)
        {
            direction = Random.Range(0, 2);
        }
    
        rotationTime += Random.Range(-rngRotation, rngRotation);
        rotationSpeed = 360f / rotationTime;

        if (anim)
        {
            anim.SetFloat("SpeedMultiplier", (direction == 0 ? -1 : 1) * (8f / rotationTime));
        }
    }

    public void StopRotation()
    {
        anim.SetFloat("SpeedMultiplier", 0);
    }
}