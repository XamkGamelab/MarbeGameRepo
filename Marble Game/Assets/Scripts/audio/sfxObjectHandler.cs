using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxObjectHandler : MonoBehaviour
{
    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine("isStillPlaying");
    }

    private IEnumerator isStillPlaying()
    {
        yield return new WaitForSeconds(1f);
        while (source.isPlaying)
        {
            yield return new WaitForSeconds(1f);
        }
        Destroy(gameObject);
    }
}
