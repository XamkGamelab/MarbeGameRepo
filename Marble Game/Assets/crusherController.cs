using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crusherController : MonoBehaviour
{
    [SerializeField] private LayerMask crushLayer;
    [SerializeField] private Animator animator;
    private bool wasUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((crushLayer.value & (1 << other.gameObject.layer)) > 0 && !wasUsed)
        {
            animator.Play("Crush");
            Destroy(other.gameObject);
            GameManager.Management.curXp += GameManager.Management.nextLevelXp / 6;
            wasUsed = true;
        }
    }
}
