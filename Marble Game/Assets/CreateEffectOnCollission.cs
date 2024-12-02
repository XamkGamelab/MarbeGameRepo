using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class CreateEffectOnCollision : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject effect;
    [SerializeField] private Transform effectSpot;
    [SerializeField] private bool Trigger = false;
    [SerializeField] private bool singleActivation;

    private bool activated;

    private void Start()
    {
        if (effectSpot == null)
        {
            effectSpot = transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0 && !Trigger)
        {
            if (!singleActivation || !activated)
            {
                GameObject vfx = Instantiate(effect, effectSpot.position, quaternion.identity);
                vfx.transform.parent = null;
                activated = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0 && Trigger)
        {
            if (!singleActivation || !activated)
            {
                GameObject vfx = Instantiate(effect, effectSpot.position, quaternion.identity);
                vfx.transform.parent = null;
                activated = true;
            }
        }
    }
}
