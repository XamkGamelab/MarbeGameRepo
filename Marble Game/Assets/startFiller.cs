using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class startFiller : MonoBehaviour
{
    public int sizeH, sizeV;
    private Tilemap map;
    public Tile tile;
    public static startFiller filler {get; private set;}

    private void Awake()
    {
        if (filler == null)
        {
            filler = this;
        }
    }

    private void Start()
    {
        map = GetComponent<Tilemap>();
        fillCanvas();
    }

    public void fillCanvas()
    {
        map.CompressBounds();
        map.SetTile(new Vector3Int(Mathf.CeilToInt(sizeH/2),sizeV-5,0), tile);
        map.SetTile(new Vector3Int(-Mathf.FloorToInt(sizeH/2),-5,0), tile);
        //map.BoxFill(new Vector3Int(0,0,0), tile, 0, -2, 5, 10);
        map.FloodFill(new Vector3Int(0,0,0), tile);
    }

    public void eraseTile(Vector3 erasePos)
    {
        Vector3Int cellPosition = map.WorldToCell(erasePos);
        map.SetTile(new Vector3Int(Mathf.RoundToInt(cellPosition.x), Mathf.RoundToInt(cellPosition.y), 0), null);
    }
}
