using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("Progression Variables")]
    public int level;
    public int xp;

    //constructor, ran on starting new game
    public GameData()
    {
        this.level = 1;
        this.xp = 0;
    }
}
