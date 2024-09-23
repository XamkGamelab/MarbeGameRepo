using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Management {get; private set;}
    
    [Header("Saved Values")]
    public int level;
    public float curXp;
    public float shards;
    
    [Header("Public Values")]
    public float nextLevelXp;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private RectTransform xpBar;
    
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
        lvlText.text = level.ToString();

        if (curXp >= nextLevelXp)
        {
            curXp -= nextLevelXp;
            level++;
        }

        xpBar.sizeDelta = new Vector3((curXp/nextLevelXp)* 1000, xpBar.sizeDelta.y);
    }
}