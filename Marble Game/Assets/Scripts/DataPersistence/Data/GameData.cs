using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    [Serializable]
    public struct JsonDateTime
    {
        public long value;
        public static implicit operator DateTime(JsonDateTime jdt)
        {
            return DateTime.FromFileTime(jdt.value);
        }
        public static implicit operator JsonDateTime(DateTime dt)
        {
            JsonDateTime jdt = new JsonDateTime();
            jdt.value = dt.ToFileTime();
            return jdt;
        }
    }

    [Header("Progression Variables")]
    public int level;
    public float xp;
    public int shards;
    public float xpModifier;
    public List<bool> skinOwned;
    public int skinsAmount;
    public int activeSkin;
    public int tutsSeen;
    public string jsonLastOpened;

    //constructor, ran on starting new game
    public GameData()
    {
        this.level = 0;
        this.xp = 0;
        this.shards = 0;
        this.xpModifier = 1;
        this.skinsAmount = 0;
        this.activeSkin = 2;
        this.tutsSeen = 0;
        jsonLastOpened = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
    }
}
