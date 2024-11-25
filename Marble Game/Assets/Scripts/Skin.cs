using System;
using TMPro.Examples;
using UnityEngine;

[System.Serializable]
public class Skin
{
    public enum SkinName
    {
        Beach,
        Catten,
        Classic,
        Kitten,
        Magic,
        Relic,
        Voronoi
    }

    public enum Rarity
    {
        Common,
        Rare,
        Misc
    }

    [field: SerializeField] public int id { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public SkinName skinName { get; private set; }
    [field: SerializeField] public Rarity rarity { get; set; }
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public bool owned { get; set; }
}
