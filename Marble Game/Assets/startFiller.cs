using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class startFiller : MonoBehaviour
{
    [Header("DevOnly")]
    [SerializeField] private bool overWriteGeneration;

    private int playerLevel;
    [Header("Scale")]
    public int sizeH, sizeV;
    [SerializeField] private int vertOffset;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap floorMap;
    
    [Header("Tiles")]
    public Tile wallTile, floorTile;
    public static startFiller filler {get; private set;}
    
    [Header("Walker Data")]
    public int walkerCount;
    [SerializeField] private GameObject walkerObj;
    [SerializeField] private Transform walkerPos;
    
    [SerializeField] private float walkerUpChance, walkerSideChance, walkerDownChance;
    [SerializeField] private float walkerDeathChance;
    [SerializeField] private int walkerMinMoves;

    public int remainingWalkers = -1;
    [Header("Goal")]
    [SerializeField] private GameObject goal;
    
    [Header("Obstacles")]
    [SerializeField] private int minObstacles, maxObstacles, curObstacles;
    [SerializeField] private float obstacleChance, obstacleMinChance, obstacleChanceMultiplier;
    [SerializeField] private GameObject[] easyObstacles;
    [SerializeField] private float easyChance;
    [SerializeField] private GameObject[] moderateObstacles;
    [SerializeField] private float moderateChance;
    [SerializeField] private GameObject[] hardObstacles;
    [SerializeField] private float hardChance;

    private void Awake()
    {
        if (filler == null)
        {
            filler = this;
        }
    }

    private void Start()
    {
        generateMap();
    }

    private void Update()
    {
        //If no remaining walkers, generate floor and goal
        if (remainingWalkers == 0)
        {
            remainingWalkers = -1;
            placeGoal();
            floorCanvas();
            placeObstacles();
        }
        
        //Debug commands
        if (Input.GetKeyDown(KeyCode.R)) //Regenerates
        {
            generateMap();
        }
    }

    private void generateMap()
    {
        //Destroy EndDestroy objects, such as the goal
        GameObject[] layerObjects = GameObject.FindGameObjectsWithTag("EndDestroy");
        foreach (GameObject layerObject in layerObjects)
        {
            Destroy(layerObject);
        }
        
        //Delete map and regenerate
        curObstacles = 0;
        deleteMap();
        playerLevel = xpManager.Management.level;
        if (!overWriteGeneration)
        {
            calculateSettings();
        }
        
        fillCanvas();
        engageWalkers();
    }
    
    
    private void calculateSettings()
    {
        sizeH = 8 + Mathf.FloorToInt((playerLevel * 0.4f))*2;
        sizeV = 20 + Mathf.FloorToInt((playerLevel * 0.6f))*2;
        
        walkerCount = Mathf.Clamp(Mathf.FloorToInt(playerLevel/10), 2, Mathf.FloorToInt((playerLevel * 1.1f)/2));
        walkerMinMoves = 20 + playerLevel*2;
        walkerDeathChance = 1f-Mathf.Clamp(playerLevel*0.005f, 0, 0.5f);
        walkerSideChance = 20 + Mathf.Clamp(Mathf.FloorToInt(playerLevel/4), 0, 20+Mathf.Clamp(Mathf.FloorToInt(playerLevel*0.2f), 0, 20));
        walkerDownChance = 5 + Mathf.Clamp(Mathf.FloorToInt(playerLevel/4), 0, 25);
        
        
        minObstacles = Mathf.CeilToInt(playerLevel/4);
        maxObstacles = Mathf.CeilToInt(playerLevel * 1.5f);
        easyChance = xpManager.Management.level;
        moderateChance = Mathf.Clamp(Mathf.FloorToInt(playerLevel/3), 0, 75);
        hardChance = Mathf.Clamp(Mathf.FloorToInt(playerLevel/6), 0, 50);
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
        remainingWalkers = 0;
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
            remainingWalkers++;
        }
    }
    
    //Generates floor where there arent any walls
    public void floorCanvas()
    {
        floorMap.CompressBounds();
        for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
        {
            for (int y = -vertOffset+1; y < sizeV-vertOffset; y++)
            {
                if (!checkIsTile(new Vector3Int(x, y, 0)))
                {
                    floorMap.SetTile(new Vector3Int(x, y,0), floorTile);
                }
            }
        }
    }

    public void placeGoal()
    {
        
        //Find highest up available tile
        int highestAvailable = 0;
        for (int y = sizeV-vertOffset; y > -vertOffset+1; y--)
        {
            for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
            {
                if (!checkIsTile(new Vector3Int(x, y, 0)))
                {
                    highestAvailable = y;
                    goto FOUND;
                }
            }
        }
        FOUND:
        
        //Randomize a tile between highest tile and the 4 below it
        int finalY = Random.Range(highestAvailable, highestAvailable - 3);
        int howManyOptions = 0;
        
        //Check how many tiles on x in the y layer
        for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
        {
            if (!checkIsTile(new Vector3Int(x, finalY, 0)))
            {
                howManyOptions++;
            }
        }
        
        //Randomize which X to use
        int finalX = Random.Range(0, howManyOptions);
        int i = 0;
        
        //Go through the x layer again, this time placing the flag at the finalX tile
        for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
        {
            if (!checkIsTile(new Vector3Int(x, finalY, 0)))
            {
                if (finalX == i)
                {
                    Instantiate(goal, new Vector3(x, finalY, 0), quaternion.identity);
                    eraseTile(new Vector3(x, finalY));
                    eraseTile(new Vector3(x, finalY+1));
                    eraseTile(new Vector3(x, finalY-1));
                    eraseTile(new Vector3(x+1, finalY));
                    eraseTile(new Vector3(x-1, finalY));
                    eraseTile(new Vector3(x+1, finalY+1));
                    eraseTile(new Vector3(x+1, finalY-1));
                    eraseTile(new Vector3(x-1, finalY+1));
                    eraseTile(new Vector3(x-1, finalY-1));
                    return;
                }
                else
                {
                    i++;
                }
                    
            }
        }
    }
    
    
    
    //Handles obstacle generations similarly to goal placement
    public void placeObstacles()
    {
        while (curObstacles < minObstacles)
        {
            float remainingChance = obstacleChance;
            for (int y = sizeV-vertOffset; y > -vertOffset+1; y--)
            {
                for (int x = -Mathf.FloorToInt(sizeH/2); x < Mathf.CeilToInt(sizeH/2); x++)
                {
                    if (!checkIsTile(new Vector3Int(x, y, 0)))
                    {
                        int genAnythingRng = Random.Range(0, 101);
                        if (remainingChance >= genAnythingRng)
                        {
                            int rngTier = Random.Range(0, 3);
                            float rng = Random.Range(1, 101);
                            if (rngTier == 0 && easyChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, easyObstacles.Length);
                                Instantiate(easyObstacles[rngEnemy], new Vector3(x, y+1, 0), quaternion.identity);
                                Debug.Log("Easy tile at: " + x +", " + y);
                                curObstacles++;
                            }
                            if (rngTier == 1 && moderateChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, moderateObstacles.Length);
                                Instantiate(moderateObstacles[rngEnemy], new Vector3(x, y+1, 0), quaternion.identity);
                                curObstacles++;
                            }
                            if (rngTier == 2 && hardChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, hardObstacles.Length);
                                Instantiate(hardObstacles[rngEnemy], new Vector3(x, y+1, 0), quaternion.identity);
                                curObstacles++;
                            }
                            
                            
                            if (remainingChance > obstacleMinChance)
                            {
                                remainingChance *= obstacleChanceMultiplier;
                            }
                            if (remainingChance < obstacleMinChance)
                            {
                                remainingChance = obstacleMinChance;
                            }
                        }
                    }
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
