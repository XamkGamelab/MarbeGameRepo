using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playCollisionAnimation : MonoBehaviour
{
    [SerializeField] private bool trigger;
    [SerializeField] private string animationName;
    [SerializeField] private Animator anim;

    private void Start()
    {
        if (!anim)
        {
            anim = gameObject.GetComponent<Animator>();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!trigger)
        {
            anim.Play(animationName, -1, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (trigger)
        {
            anim.Play(animationName, -1, 0f);
        }
    }
}
