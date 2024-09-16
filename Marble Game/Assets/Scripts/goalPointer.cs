using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalPointer : MonoBehaviour
{
    public GameObject goalPos;
    
    void Update()
    {
        if (goalPos)
        {
            transform.right = goalPos.transform.position - transform.position;
        }
    }
}
