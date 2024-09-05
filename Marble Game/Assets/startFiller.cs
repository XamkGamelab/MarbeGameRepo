using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class startFiller : MonoBehaviour
{
    public int sizeH, sizeV;
    [SerializeField] private int vertOffset;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap floorMap;
    public Tile wallTile, floorTile;
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
        fillCanvas();
        engageWalkers();
    }

    private void Update()
    {
        //Debug commands
        if (Input.GetKeyDown(KeyCode.F)) //Adds floor
        {
            floorCanvas();
        }
        if (Input.GetKeyDown(KeyCode.R)) //Regenerates
        {
            deleteMap();
            fillCanvas();
            engageWalkers();
        }
    }

    //Fills the entire canvas with walls, generating top left and bottom right corners first
    public void fillCanvas()
    {
        wallMap.CompressBounds();
        wallMap.SetTile(new Vector3Int(Mathf.CeilToInt(sizeH/2),sizeV-vertOffset,0), wallTile);
        wallMap.SetTile(new Vector3Int(-Mathf.FloorToInt(sizeH/2),-vertOffset,0), wallTile);
        //wallMap.BoxFill(new Vector3Int(0,0,0), tile, 0, -2, 5, 10);
        wallMap.FloodFill(new Vector3Int(0,0,0), wallTile);
    }

    private void engageWalkers()
    {
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
    
    //Generates floor where there arent any walls
    public void floorCanvas()
    {
        floorMap.CompressBounds();
        for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
        {
            for (int y = -vertOffset+1; y < sizeV-vertOffset+1; y++)
            {
                if (!checkIsTile(new Vector3Int(x, y, 0)))
                {
                    floorMap.SetTile(new Vector3Int(x, y,0), floorTile);
                }
            }
        }
        
    }
    
    //Erases tile automatically converting wallMap position to tilewallMap position
    public void eraseTile(Vector3 erasePos)
    {
        Vector3Int cellPosition = wallMap.WorldToCell(erasePos);
        wallMap.SetTile(new Vector3Int(Mathf.RoundToInt(cellPosition.x), Mathf.RoundToInt(cellPosition.y), 0), null);
    }
    
    //Checks if given vector3int has a tile
    public bool checkIsTile(Vector3Int checkPos)
    {
        if (wallMap.GetTile(checkPos) == null)
        {
            return false;
        }
        return true;
    }
    
    //Checks if given vector3int is within the bounds of the wallwallMap
    public bool checkBounds(Vector3Int checkPos)
    {
        if (checkPos.x < sizeH / 2 && checkPos.x > -sizeH / 2 && checkPos.y < sizeV -vertOffset+1 && checkPos.y > -vertOffset+1)
        {
            return true;
        }
        
        return false;
    }
    
    //Destroys all maps
    public void deleteMap()
    {
        wallMap.ClearAllTiles();
        floorMap.ClearAllTiles();
        
    }
}
