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
    
    [Header("Saved Values")]
    public int level;
    public float curXp;
    public int shards;
    public float xpModifier = 1;
    [SerializeField] private float previousXpModifier; //Not actually saved, but is related to xpModifier

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

        nextLevelXp = (level*(level/2) + 1) * 100;
        xpText.text = Mathf.RoundToInt(curXp) + " / " + Mathf.RoundToInt(nextLevelXp);
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
        }

        xpBar.fillAmount  = curXp/nextLevelXp;
    }

    public void grantXp()
    {
        if (Mathf.Approximately(xpModifier, previousXpModifier))
        {
            xpModifier += xpModBonus;
        }

        float addedXP = (100 + (level * 50)) * xpModifier;
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
