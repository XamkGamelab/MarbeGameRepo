using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherDestructor : MonoBehaviour
{
    [SerializeField] private LayerMask crushLayer;
    [SerializeField] private float xpReduction;

    [SerializeField] private Transform knockbackSpot;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackRadius;

    public void HandleKnockback()
    {
        //Damage
        Vector2 VectorSpot = knockbackSpot.position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(VectorSpot, 0.75f, crushLayer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.layer == 6)
            {
                GameManager.Management.xpReduction(xpReduction);
            }
            if (hitColliders[i].gameObject.layer == 8)
            {
                hitColliders[i].gameObject.SetActive(false);
                GameManager.Management.curXp += GameManager.Management.nextLevelXp / 6;
            }
        }
        
        //Knockback
        hitColliders = Physics2D.OverlapCircleAll(VectorSpot, knockbackRadius, crushLayer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log(hitColliders[i]);
            
            
            if (hitColliders[i].GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = hitColliders[i].GetComponent<Rigidbody2D>();
                Vector2 force = (rb.position - (Vector2)knockbackSpot.position).normalized;
                float scaledForce = knockbackForce * (1 - (Vector2.Distance(rb.position, knockbackSpot.position) / knockbackRadius));
                scaledForce = Mathf.Max(0, scaledForce);
                rb.AddForce(force * scaledForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(knockbackSpot.position, knockbackRadius);
    }
}
