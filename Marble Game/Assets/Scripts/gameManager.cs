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
    
    [Header("Public Values")]
    public float nextLevelXp;

    public float xpModifier = 1;
    public bool menuOpen = true;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private TMP_Text shardText;
    [SerializeField] private Image xpBar;
    [SerializeField] private TMP_Text xpLossText;
    
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

        if (curXp >= nextLevelXp)
        {
            Debug.Log("Gained a level, and with it a Shard!");
            curXp -= nextLevelXp;
            level++;
            shards++;
        }

        xpBar.fillAmount  = curXp/nextLevelXp;
    }

    public void grantXp()
    {
        if (xpModifier < 0.1f)
        {
            xpModifier = 0.1f;
        }

        float addedXP = (100 + (level * 50)) * xpModifier;
        curXp += addedXP;
        Debug.Log("Gained: " + addedXP + " experience.");
        //Debug.Log(100-(xpModifier*100) + "% of experience was lost because of bumping into obstacles.");
        xpLossText.text = (100-(xpModifier*100)).ToString("0.0") + "% of experience was lost due to collision with foes.";
        xpModifier = 1;
    }

    //DATA PERSISTENCE
    public void LoadData(GameData gameData)
    {
        this.curXp = gameData.xp;
        this.level = gameData.level;
        this.shards = gameData.shards;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.xp = this.curXp;
        gameData.level = this.level;
        gameData.shards = this.shards;
    }
}
