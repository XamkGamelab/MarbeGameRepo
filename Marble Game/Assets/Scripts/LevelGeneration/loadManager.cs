using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class loadManager : MonoBehaviour
{
    public static loadManager Management {get; private set;}
    [Header("Variables")]
    [SerializeField] private GameObject player;
    
    [Header("UI")]
    [SerializeField] private Image fadeToBlack;

    [SerializeField] private GameObject loadingUI;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private TMP_Text generationText;
    [SerializeField] private GameObject xpLossText;
    [SerializeField] private GameObject menuButtonHolder;

    [SerializeField] private float totalDelay;
    private float curDelay;
    private bool transitionIn; //false fades out, true fades in. 

    private bool generateAfter;

    public bool isTransitioning;
    private bool canContinue;

    private bool startScreen = true;
    [SerializeField] private Image splashScreen;
    [SerializeField] private float splashScreenDelay;
    private float curDelaySplashScreen;

    [Header("Advertising")]
    [SerializeField] [Range(0, 1)] private float advertChance;
    [SerializeField] private advertController advertControl;
    
    [Header("Input System")]
    [SerializeField] private InputReader inputReader;

    [Header("Camera Management")]
    [SerializeField] private CameraBehavior cameraBehavior;

    private void Awake()
    {
        Management = this;
        splashScreen.color = new Color(1, 1, 1, 1);
    }

    private void Update()
    {
        //Handles start splash Screen
        if (startScreen == false && curDelaySplashScreen > 0)
        {
            curDelaySplashScreen -= Time.deltaTime;
            splashScreen.color = new Color(1, 1, 1, 1*curDelaySplashScreen/totalDelay);
        }
        
        
        if (curDelay > 0)
        {
            curDelay -= Time.deltaTime;
            if (transitionIn)
            {
                fadeToBlack.color = new Color(0, 0, 0, 1-1*curDelay/totalDelay);
            }
            else
            {
                fadeToBlack.color = new Color(0, 0, 0, 1*curDelay/totalDelay);
            }
        } else if (curDelay > -1)
        {
            //Enable UI
            if (transitionIn)
            {
                curDelay = -1;

                float advertRng = Random.Range(0f, 1f);
                if (advertRng <= advertChance && !startScreen)
                {
                    advertControl.startAdvert();
                }
                
                generationText.text = "Generating Level...";
                loadingUI.SetActive(true);
                xpLossText.SetActive(true);
                menuButtonHolder.SetActive(true);
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

            if (startScreen)
            {
                startScreen = false;
                curDelaySplashScreen = splashScreenDelay;
            }
        }
    }

    public void startTransitionIn()
    {
        isTransitioning = true;
        transitionIn = true;
        generateAfter = true;
        continueButton.SetActive(false);
        xpLossText.SetActive(false);
        menuButtonHolder.SetActive(false);
        canContinue = true;
        curDelay = totalDelay;
        GameManager.Management.menuOpen = true;
        inputReader.DisableGameplay();
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EndDestroy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>())
            {
                enemy.GetComponent<EnemyController>().canMove = false;
                enemy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
    }
    
    public void startTransitionOut()
    {
        isTransitioning = true;
        continueButton.SetActive(false);
        loadingUI.SetActive(false);
        xpLossText.SetActive(false);
        menuButtonHolder.SetActive(false);
        transitionIn = false;
        curDelay = totalDelay;
        GameManager.Management.menuOpen = false;
        inputReader.SetGameplay();
        cameraBehavior.DefaultOffset();
    }
    
    private IEnumerator enableContinue()
    {
        yield return new WaitForSeconds(0.2f);
        continueButton.SetActive(true);
        player.GetComponent<PlayerController>().firstMove = true;
        generationText.text = "Generation Complete!";
    }
}
