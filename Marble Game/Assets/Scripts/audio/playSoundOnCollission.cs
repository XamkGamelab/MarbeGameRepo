using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class playSoundOnCollission : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private string[] sfxName;
    private AudioSource source;
    [SerializeField] private bool Trigger = false;
    [SerializeField] private bool createSfxObject = false;

    private void Awake()
    {
        if (!createSfxObject)
        {
            if (!source)
            {
                source = gameObject.AddComponent<AudioSource>();
                source.spatialBlend = 1;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0 && !Trigger)
        {
            makeSfxObject();
            
            string randomizedString = sfxName[Random.Range(0, sfxName.Length)];
            audioManager.Management.PlayClip(source, randomizedString);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0 && Trigger)
        {
            makeSfxObject();
            
            string randomizedString = sfxName[Random.Range(0, sfxName.Length)];
            audioManager.Management.PlayClip(source, randomizedString);
        }
    }

    private void makeSfxObject()
    {
        if (createSfxObject)
        {
            GameObject sfxObject = new GameObject("sfxObject");
            sfxObject.transform.parent = null;
            source = sfxObject.AddComponent<AudioSource>();
            source.spatialBlend = 1;
            sfxObject.AddComponent<sfxObjectHandler>();
        }
    }
}
