using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteObjectsOnTop : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine("activationDelay");
    }

    private IEnumerator activationDelay()
    {
        yield return new WaitForSeconds(0.5f);
        RaycastHit2D[] intersecting = Physics2D.CircleCastAll( transform.position, 0.01f, Vector2.zero);
        if (intersecting.Length > 1)
        {
            foreach (RaycastHit2D hit in intersecting)
            {
                if (hit.collider != transform.GetComponent<Collider2D>())
                {
                    Destroy(hit.transform.gameObject);
                    Debug.Log("Destroyed: " + hit.collider.name + " because it overlapped with " + transform.name);
                }
            }
        }
        else
        {
            //Debug.Log("No garbage on top of " + transform.name);
        }
    }
}
