using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyCloseDuplicates : MonoBehaviour
{
    [SerializeField] private float range;
    void Start()
    {
        var objects = GameObject.FindObjectsOfType<destroyCloseDuplicates>();

        foreach (var obj in objects)
        {
            if (obj == this) 
                continue;
            
            if (Vector3.Distance(transform.position, obj.transform.position) < range)
            {
                Destroy(gameObject);
                Debug.Log("Destruction");
                return;
            }
        }
    }
}
