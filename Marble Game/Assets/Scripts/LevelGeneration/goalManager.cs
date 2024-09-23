using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Pointer").GetComponent<goalPointer>().goalPos = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            gameManager.Management.curXp += 100;
            startFiller.filler.generateMap();
        }
    }
}
