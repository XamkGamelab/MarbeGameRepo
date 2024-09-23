using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xpModReducer : MonoBehaviour
{
    [SerializeField] private float xpReduction; //tracked in %
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Management.xpModifier -= xpReduction;
        }
    }
}
