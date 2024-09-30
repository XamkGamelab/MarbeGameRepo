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

    private void Awake()
    {
        if (!source)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0)
        {
            string randomizedString = sfxName[Random.Range(0, sfxName.Length)];
            audioManager.Management.PlayClip(source, randomizedString);
        }
    }
}
