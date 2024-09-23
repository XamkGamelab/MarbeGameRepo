using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class loadManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fadeToBlack;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private TMP_Text generationText;
    [SerializeField] private RectTransform loadBar;

    [SerializeField] private float delayBeforeUI;
    [SerializeField] private float curDelay;
    [SerializeField] private bool transitionIn; //false fades out, true fades in. 

    public bool isTransitioning;

    private void Update()
    {
        if (curDelay > 0)
        {
            if (transitionIn)
            {
                //Currently black screen
                fadeToBlack.color = new Color(0, 0, 0, 1-0);
            }
            else
            {
                //Currently black screen
                fadeToBlack.color = new Color(0, 0, 0, 0);
            }
        } else if (curDelay > -1)
        {
            curDelay = -1;
        }
        
        /*
        if (fadeTimer <= 0 && fadeIn)
        {
            fadeIn = false;
            generation();
        }*/
    }
}
