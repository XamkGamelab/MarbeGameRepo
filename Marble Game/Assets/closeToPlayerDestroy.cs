using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closeToPlayerDestroy : MonoBehaviour
{
    [SerializeField] private float range;

    private void Start()
    {
        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < range)
        {
            Destroy(gameObject);
        }
    }
}
