using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager Management {get; private set;}
    private const float xpCurveScaler = 100; //lower to ease requirements, higher to increase. default 100.
    private const int shardBonusFrequency = 5; //on reaching a level divisible by this number, player gets
                                               //bonus shards equal to below integer
    private const int shardBonusAmount = 0; //this is added on top of the normal +1 shard on levelup
                                            //after hitting a level divisible by above integer. default 4.
    
    [Header("Saved Values")]
    public int level;
    public float curXp;
    public int shards;
    public float xpModifier = 1;
    private float previousXpModifier; //Not actually saved, but is related to xpModifier.

    [Header("Serialized Values")]
    [SerializeField] private float xpModBonus;
    
    [Header("Public Values")]
    public float nextLevelXp;
    
    public bool menuOpen = true;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private TMP_Text shardText;
    [SerializeField] private Image xpBar;
    [SerializeField] private TMP_Text modifierText;
    [SerializeField] private TMP_Text xpModLossText;
    [SerializeField] private Animator xpModLossAnim;

    [SerializeField] private ParticleSystem levelUpVfx;
    
    [Header("Funky Titles")]
    [SerializeField] private string[] titles;
    private void Awake()
    {
        if (Management == null)
        {
            Management = this;
        }
    }

    private void Update()
    {
        if (level < titles.Length & level >= 0)
        {
            titleText.text = titles[level];
        }

        float clampedLvl = Mathf.Min(level, 20);
        
        nextLevelXp = (clampedLvl*(clampedLvl/2) + 1) * xpCurveScaler;
        xpText.text = Mathf.FloorToInt(curXp) + " / " + Mathf.RoundToInt(nextLevelXp);
        lvlText.text = (level+1).ToString();
        shardText.text = shards.ToString();

        if (xpModifier >= 1)
        {
            modifierText.text = "+" + Mathf.RoundToInt((xpModifier-1) * 100).ToString() +"%";
        }
        else
        {
            modifierText.text = Mathf.RoundToInt((xpModifier-1) * 100).ToString() +"%";
        }

        if (curXp >= nextLevelXp)
        {
            curXp -= nextLevelXp;
            level++;
            shards++;
            if (level % shardBonusFrequency == 0)
            {
                shards += shardBonusAmount;
            }

            levelUpVfx.Play();
        }

        xpBar.fillAmount  = curXp/nextLevelXp;
    }

    public void grantXp()
    {
        if (Mathf.Approximately(xpModifier, previousXpModifier))
        {
            xpModifier += xpModBonus;
        }

        float addedXP = (100 + (Mathf.Min(level, 20) * 50)) * xpModifier;
        curXp += addedXP;
        
        if (xpModifier < 1)
        {
            xpModifier = 1;
        }

        previousXpModifier = xpModifier;
    }

    public void xpReduction(float reduction)
    {
        if (xpModifier > 1)
        {
            if (xpModifier-reduction < 1)
            {
                reduction = xpModifier - 1;
                xpModifier = 1;
                xpLossAnim(reduction);
            }
            else
            {
                xpModifier -= reduction*2;
                xpLossAnim(reduction*2);
            }
        }
        else
        {
            xpModifier -= reduction;
            xpLossAnim(reduction);
        }

        if (xpModifier < 0.1)
        {
            xpModifier = 0.1f;
        }
    }

    private void xpLossAnim(float reduction)
    {
        xpModLossText.text = "-" + Mathf.RoundToInt(reduction * 100).ToString() + "%";
        xpModLossAnim.Play("Activated", -1, 0f);
        xpModLossAnim.Play("Activated");
    }

    //DATA PERSISTENCE
    public void LoadData(GameData gameData)
    {
        this.curXp = gameData.xp;
        this.level = gameData.level;
        this.shards = gameData.shards;
        this.xpModifier = gameData.xpModifier;
        previousXpModifier = xpModifier;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.xp = this.curXp;
        gameData.level = this.level;
        gameData.shards = this.shards;
        gameData.xpModifier = this.xpModifier;
    }
}
