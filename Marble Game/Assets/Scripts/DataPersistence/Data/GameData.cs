using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [Header("Progression Variables")]
    public int level;
    public float xp;

    //constructor, ran on starting new game
    public GameData()
    {
        this.level = 0;
        this.xp = 0;
    }
}
