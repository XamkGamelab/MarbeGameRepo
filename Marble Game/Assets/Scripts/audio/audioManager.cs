using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Management {get; private set;}
    public LabeledAudioClip[] audioClips;
    
    private void Awake()
    {
        if (Management == null)
        {
            Management = this;
        }
    }

    public void PlayClip(AudioSource source, string label)
    {
        foreach (LabeledAudioClip labeledClip in audioClips)
        {
            if (labeledClip.label == label)
            {
                source.PlayOneShot(labeledClip.clip);
                return;
            }
        }
        
        Debug.LogWarning("Audio clip with label " + label + " not found.");
    }
}

[System.Serializable]
public class LabeledAudioClip
{
    public string label;
    public AudioClip clip;
}