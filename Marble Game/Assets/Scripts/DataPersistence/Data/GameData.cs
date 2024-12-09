using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [Header("Progression Variables")]
    public int level;
    public float xp;
    public int shards;
    public float xpModifier;
    public List<bool> skinOwned;
    public int skinsAmount;
    public int activeSkin;
    public bool tutSeen;
    public bool enemyTutSeen;
    public DateTime lastOpenedTime;

    //constructor, ran on starting new game
    public GameData()
    {
        this.level = 0;
        this.xp = 0;
        this.shards = 0;
        this.xpModifier = 1;
        this.skinsAmount = 0;
        this.activeSkin = 2;
        this.tutSeen = false;
        this.enemyTutSeen = false;
        lastOpenedTime = DateTime.Now;
    }
}
