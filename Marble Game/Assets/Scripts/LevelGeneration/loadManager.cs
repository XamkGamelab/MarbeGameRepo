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

    private PlayerController playerControl;
    
    [Header("UI")]
    [SerializeField] private Image fadeToBlack;

    [SerializeField] private GameObject loadingUI;

    [SerializeField] private GameObject continueButton;
    [SerializeField] private TMP_Text generationText;
    [SerializeField] private GameObject xpLossText;
    [SerializeField] private GameObject menuButtonHolder;
    [SerializeField] private GameObject buttonShadow;

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

    [SerializeField] private slowRBVelocity slowdownScript;

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
        splashScreen.color = new Color(splashScreen.color.r, splashScreen.color.g, splashScreen.color.b, 1);
        playerControl = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        //Handles start splash Screen
        if (startScreen == false && curDelaySplashScreen > 0)
        {
            curDelaySplashScreen -= Time.deltaTime;
            splashScreen.color = new Color(splashScreen.color.r, splashScreen.color.g, splashScreen.color.b, 1*curDelaySplashScreen/totalDelay);
        }
        
        
        if (curDelay > 0)
        {
            curDelay -= Time.deltaTime;
            if (transitionIn)
            {
                fadeToBlack.color = new Color(fadeToBlack.color.r, fadeToBlack.color.g, fadeToBlack.color.b, 1-1*curDelay/totalDelay);
            }
            else
            {
                fadeToBlack.color = new Color(fadeToBlack.color.r, fadeToBlack.color.g, fadeToBlack.color.b, 1*curDelay/totalDelay);
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
        buttonShadow.SetActive(false);
        canContinue = true;
        curDelay = totalDelay;
        GameManager.Management.menuOpen = true;
        inputReader.DisableGameplay();
        slowdownScript.eradicateList();
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EndDestroy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>())
            {
                enemy.GetComponent<EnemyController>().canMove = false;
                enemy.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            if (enemy.GetComponent<StableRotation>())
            {
                enemy.GetComponent<StableRotation>().StopRotation();
            }
        }
    }
    
    public void startTransitionOut()
    {
        generationText.text = "";
        isTransitioning = true;
        continueButton.SetActive(false);
        loadingUI.SetActive(false);
        xpLossText.SetActive(false);
        menuButtonHolder.SetActive(false);
        buttonShadow.SetActive(false);
        transitionIn = false;
        curDelay = totalDelay;
        GameManager.Management.menuOpen = false;
        inputReader.SetGameplay();
        cameraBehavior.DefaultOffset();
        playerControl.resetConditions();
    }

    public void dataDeletion()
    {
        generationText.text = "Generating Level...";
        continueButton.SetActive(false);
        buttonShadow.SetActive(false);
        canContinue = true;
    }
    
    private IEnumerator enableContinue()
    {
        yield return new WaitForSeconds(0.2f);
        continueButton.SetActive(true);
        buttonShadow.SetActive(true);
        playerControl.firstMove = true;
        if (startScreen)
        {
            generationText.text = "";
            startScreen = false;
            curDelaySplashScreen = splashScreenDelay;
        }
        else
        {
            generationText.text = "Generation Complete!"; 
        }
        playerControl.resetConditions();
    }
}
