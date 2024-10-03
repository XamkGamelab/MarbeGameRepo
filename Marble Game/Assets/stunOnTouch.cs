using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunOnTouch : MonoBehaviour
{
    [SerializeField] private float stunDur;

    private void Awake()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerController>().stunPlayer(stunDur, false);
        }
    }
}
