using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class continousMovementSound : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] private float maxSpeed;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    
    private AudioSource source;
    private Rigidbody2D rb;

    private void Start()
    {
        if (!source)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.loop = true;
            source.spatialBlend = 1;
            source.playOnAwake = false;
        }
        rb = GetComponent<Rigidbody2D>();
        
        foreach (LabeledAudioClip labeledClip in audioManager.Management.audioClips)
        {
            if (labeledClip.label == label)
            {
                source.clip = labeledClip.clip;
                source.Play();
                return;
            }
        }
    }

    private void Update()
    {
        if (rb.velocity.magnitude > 0)
        {
            source.volume = (rb.velocity.magnitude/maxSpeed)*volume;
        }
        else
        {
            source.volume = 0;
        }
    }
}
