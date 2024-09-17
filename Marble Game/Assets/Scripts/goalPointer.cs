using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalPointer : MonoBehaviour
{
    [SerializeField] private GameObject playerPos;
    public GameObject goalPos;
    
    void Update()
    {
        if (goalPos)
        {
            transform.right = goalPos.transform.position - playerPos.transform.position;
        }
    }
}
