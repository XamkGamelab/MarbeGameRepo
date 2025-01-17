using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalManager : MonoBehaviour
{
    private bool goalActivated = false;
    void Start()
    {
        GameObject.FindGameObjectWithTag("Pointer").GetComponent<goalPointer>().goalPos = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") && !goalActivated)
        {
            goalActivated = true;
            GameManager.Management.grantXp();
            loadManager.Management.startTransitionIn();

            other.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }
}
