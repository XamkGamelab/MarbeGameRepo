using System;
using TMPro.Examples;
using UnityEngine;

[Serializable]
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

    [field: SerializeField] public int id { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public SkinName skinName { get; private set; }
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public bool owned { get; set; }
}
