using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowFollow : MonoBehaviour
{
    private GameObject parent;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        parent = transform.parent.gameObject;
        transform.parent = null;
    }

    private void Update()
    {

        if (!parent)
        {
            Destroy(gameObject);
        }
        else
        transform.position = parent.transform.position + offset;
    }
}
