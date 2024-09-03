using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class startFiller : MonoBehaviour
{
    public int sizeH, sizeV;
    private Tilemap map;
    public Tile tile;
    public static startFiller filler {get; private set;}

    public int walkerCount;
    [SerializeField] private GameObject walkerObj;
    [SerializeField] private Transform walkerPos;
    
    [SerializeField] private float walkerUpChance, walkerSideChance, walkerDownChance;
    [SerializeField] private float walkerDeathChance;
    [SerializeField] private int walkerMinMoves;

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
        
        //Creates walkers to erode walls
        for (int i = 0; i < walkerCount; i++)
        {
            GameObject newWalker = Instantiate(walkerObj, walkerPos.position, quaternion.identity);
            walker walkerScript = walkerObj.GetComponent<walker>();
            walkerScript.upChance = walkerUpChance;
            walkerScript.sideChance = walkerSideChance;
            walkerScript.downChance = walkerDownChance;
            walkerScript.deathChance = walkerDeathChance;
            walkerScript.minMoves = walkerMinMoves;
        }
    }
    
    //Fills the entire canvas with walls, generating top left and bottom right corners first
    public void fillCanvas()
    {
        map.CompressBounds();
        map.SetTile(new Vector3Int(Mathf.CeilToInt(sizeH/2),sizeV-5,0), tile);
        map.SetTile(new Vector3Int(-Mathf.FloorToInt(sizeH/2),-5,0), tile);
        //map.BoxFill(new Vector3Int(0,0,0), tile, 0, -2, 5, 10);
        map.FloodFill(new Vector3Int(0,0,0), tile);
    }
    
    //Erases tile automatically converting map position to tilemap position
    public void eraseTile(Vector3 erasePos)
    {
        Vector3Int cellPosition = map.WorldToCell(erasePos);
        map.SetTile(new Vector3Int(Mathf.RoundToInt(cellPosition.x), Mathf.RoundToInt(cellPosition.y), 0), null);
    }
    
    //Checks if given vector3int has a tile
    public bool checkIsTile(Vector3Int checkPos)
    {
        if (map.GetTile(checkPos) == null)
        {
            return false;
        }
        return true;
    }
    
    //Checks if given vector3int is within the bounds of the wallmap
    public bool checkBounds(Vector3Int checkPos)
    {
        if (checkPos.x < sizeH / 2 && checkPos.x > -sizeH / 2 && checkPos.y < sizeV -4 && checkPos.y > -4)
        {
            return true;
        }
        
        return false;
    }
}
