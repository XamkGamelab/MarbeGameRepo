using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeOnTouch : MonoBehaviour
{
    [SerializeField] private float frostDur;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.transform.GetComponent<PlayerController>().freezePlayer(frostDur, false);
        }
    }
}
