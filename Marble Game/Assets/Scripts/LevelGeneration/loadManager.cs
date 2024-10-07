using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class loadManager : MonoBehaviour
{
    public static loadManager Management {get; private set;}
    
    [Header("UI")]
    [SerializeField] private Image fadeToBlack;

    [SerializeField] private GameObject loadingUI;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private TMP_Text generationText;

    [SerializeField] private float totalDelay;
    private float curDelay;
    private bool transitionIn; //false fades out, true fades in. 

    private bool generateAfter;

    public bool isTransitioning;
    private bool canContinue;

    private void Awake()
    {
        Management = this;
    }

    private void Update()
    {
        if (curDelay > 0)
        {
            curDelay -= Time.deltaTime;
            if (transitionIn)
            {
                //Currently black screen
                fadeToBlack.color = new Color(0, 0, 0, 1-1*curDelay/totalDelay);
            }
            else
            {
                //Currently black screen
                fadeToBlack.color = new Color(0, 0, 0, 1*curDelay/totalDelay);
            }
        } else if (curDelay > -1)
        {
            curDelay = -1;
            
            //Enable UI
            if (transitionIn)
            {
                loadingUI.SetActive(true);
            }
            
            //Start finishing generation
            if (generateAfter)
            {
                startFiller.filler.mapGeneration();
                generateAfter = false;

                //save game
                DataPersistenceManager.instance.SaveGame();
            }
            
            isTransitioning = false;
        }

        if (startFiller.filler.remainingWalkers <= 0 && !isTransitioning && canContinue)
        {
            StartCoroutine("enableContinue");
            canContinue = false;
        }
    }

    public void startTransitionIn()
    {
        isTransitioning = true;
        transitionIn = true;
        generateAfter = true;
        continueButton.SetActive(false);
        canContinue = true;
        curDelay = totalDelay;
    }
    
    public void startTransitionOut()
    {
        isTransitioning = true;
        continueButton.SetActive(false);
        loadingUI.SetActive(false);
        transitionIn = false;
        curDelay = totalDelay;
    }
    
    private IEnumerator enableContinue()
    {
        yield return new WaitForSeconds(0.2f);
        continueButton.SetActive(true);
    }
}
