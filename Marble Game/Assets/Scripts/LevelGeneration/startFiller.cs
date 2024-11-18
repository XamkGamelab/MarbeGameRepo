using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.EventSystems;

public class startFiller : MonoBehaviour
{
    [Header("DevOnly")]
    [SerializeField] private bool overWriteGeneration;

    private GameObject player;

    private int playerLevel;
    [Header("Scale")]
    
    public int sizeH, sizeV;
    [SerializeField] private int vertOffset;

    public int walkerBounds;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private Tilemap floorMap;
    
    [Header("Tiles")]

    public RuleTile wallTile, floorTile;
    public static startFiller filler {get; private set;}
    
    [Header("Walker Data")]
    public int walkerCount;
    [SerializeField] private GameObject walkerObj;
    [SerializeField] private GameObject randomWalker;
    [SerializeField] private Transform walkerPos;
    
    [SerializeField] private float walkerUpChance, walkerSideChance, walkerDownChance, walkerSpawnChance;
    [SerializeField] private float walkerDeathChance;
    [SerializeField] private int walkerMinMoves;

    public int remainingWalkers = -1;
    [Header("Goal")]
    [SerializeField] private GameObject goal;

    private Vector2 curFlagPos;
    
    [Header("Obstacles")]
    [SerializeField] private int minObstacles, maxObstacles, curObstacles;
    [SerializeField] private float obstacleChance, obstacleMinChance, obstacleChanceMultiplier;
    [SerializeField] private GameObject[] easyObstacles;
    [SerializeField] private float easyChance;
    [SerializeField] private GameObject[] moderateObstacles;
    [SerializeField] private float moderateChance;
    [SerializeField] private GameObject[] hardObstacles;
    [SerializeField] private float hardChance;

    private bool openedGame = true;
    private void Awake()
    {
        if (filler == null)
        {
            filler = this;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        
        //Ensures no weird start sounds, etc
        if (!overWriteGeneration)
        {
            sizeH = 0;
            sizeV = 0;
        
            walkerCount = 0;
            walkerMinMoves = 0;
            walkerDeathChance = 0;
            walkerSideChance = 0;
            walkerDownChance = 0;
            walkerSpawnChance = 0;
        
        
            minObstacles = 0;
            maxObstacles = 0;
            easyChance = 0;
            moderateChance = 0;
            hardChance = 0;
        }
    }

    private void Start()
    {
        loadManager.Management.startTransitionIn();
    }

    private void Update()
    {
        //If no remaining walkers, generate floor and goal
        if (remainingWalkers == 0 && openedGame)
        {
            remainingWalkers = -1;
            placeGoal();
            placeObstacles();
        }
        
        //Debug commands
        if (Input.GetKeyDown(KeyCode.R)) //Regenerates
        {
            loadManager.Management.startTransitionIn();
        }
        
        //Object check debugger. Keep commented when not in use.
        /*
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D intersecting = Physics2D.CircleCast( Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.01f, Vector2.zero);
            if (intersecting)
            {
                Debug.Log(intersecting.collider.name);
            }
            else
            {
                Debug.Log("No collission detected");
            }

        }
        */
    }

    public void mapGeneration()
    {
        //Reset player
        player.transform.position = walkerPos.transform.position;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        
        //Destroy EndDestroy objects, such as the goal
        GameObject[] layerObjects = GameObject.FindGameObjectsWithTag("EndDestroy");
        foreach (GameObject layerObject in layerObjects)
        {
            Destroy(layerObject);
        }
        
        //Delete map and regenerate
        curObstacles = 0;
        deleteMap();
        playerLevel = GameManager.Management.level;
        if (!overWriteGeneration)
        {
            calculateSettings();
        }
        
        fillCanvas();
        engageWalkers();
    }
    
    
    private void calculateSettings()
    {
        sizeH = 44 + Mathf.FloorToInt((playerLevel * 0.4f))*2;
        sizeV = 56 + Mathf.FloorToInt((playerLevel * 0.6f))*2;
        
        walkerCount = Mathf.Clamp(Mathf.FloorToInt(playerLevel/10), 2, 20);
        walkerMinMoves = 40 + playerLevel*2;
        walkerDeathChance = 1f-Mathf.Clamp(playerLevel*0.005f, 0, 0.5f);
        walkerSideChance = 20 + Mathf.Clamp(Mathf.FloorToInt(playerLevel/4)+5, 0, 30+Mathf.Clamp(Mathf.FloorToInt(playerLevel*0.2f), 0, 30));
        walkerDownChance = 5 + Mathf.Clamp(Mathf.FloorToInt(playerLevel/4)+5, 0, 35);
        walkerSpawnChance = Mathf.Clamp(playerLevel * 0.3f+5, 0, 40);
        
        obstacleChance = Mathf.Clamp(GameManager.Management.level+50, 0, 80);
        minObstacles = Mathf.CeilToInt(playerLevel/6);
        if (playerLevel > 2)
        {
            minObstacles += 2;
        }

        if (playerLevel > 20)
        {
            minObstacles += 4;
        }
        maxObstacles = Mathf.CeilToInt(playerLevel * 1.5f);
        easyChance = 100;
        moderateChance = Mathf.Clamp(Mathf.FloorToInt(playerLevel/3), 0, 75);
        hardChance = Mathf.Clamp(Mathf.FloorToInt(playerLevel/6), 0, 50);
    }
    

    //Fills the entire canvas with walls, generating top left and bottom right corners first
    public void fillCanvas()
    {
        wallMap.CompressBounds();
        wallMap.SetTile(new Vector3Int(Mathf.CeilToInt(sizeH/2),sizeV-vertOffset,0), wallTile);
        wallMap.SetTile(new Vector3Int(-Mathf.FloorToInt(sizeH/2),-vertOffset,0), wallTile);
        wallMap.FloodFill(Vector3Int.zero, wallTile);
    }

    private void engageWalkers()
    {
        remainingWalkers = 1;
        Instantiate(randomWalker, walkerPos.position, quaternion.identity);
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
            walkerScript.newWalkerSpawn = walkerSpawnChance;
            remainingWalkers++;
        }
    }
    
    //Generates floor where there arent any walls
    private IEnumerator floorCanvas()
    {
        yield return new WaitForSeconds(0.1f);

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
        // Find highest up available tile
        int highestAvailable = 0;
        for (int y = sizeV - vertOffset; y > -vertOffset + 1; y--)
        {
            for (int x = -Mathf.FloorToInt(sizeH / 2); x < Mathf.CeilToInt(sizeH / 2); x++)
            {
                if (!checkIsTile(new Vector3Int(x, y, 0)))
                {
                    highestAvailable = y;
                    goto FOUND;
                }
            }
        }
        FOUND:

        int finalY = Random.Range(highestAvailable, highestAvailable - 3);
        int howManyOptions = 0;

        for (int x = -Mathf.FloorToInt(sizeH / 2); x < Mathf.CeilToInt(sizeH / 2); x++)
        {
            if (!checkIsTile(new Vector3Int(x, finalY, 0)))
            {
                howManyOptions++;
            }
        }

        int finalX = Random.Range(0, howManyOptions);
        int i = 0;

        for (int x = -Mathf.FloorToInt(sizeH / 2); x < Mathf.CeilToInt(sizeH / 2); x++)
        {
            if (!checkIsTile(new Vector3Int(x, finalY, 0)))
            {
                if (finalX == i)
                {
                    Vector3 flagPosition = new Vector3(x, finalY, 0);
                    Instantiate(goal, flagPosition, quaternion.identity);
                    curFlagPos = new Vector2(flagPosition.x, flagPosition.y); // Store the flag position

                    eraseTile(new Vector3(x, finalY));
                    eraseTile(new Vector3(x, finalY + 1));
                    eraseTile(new Vector3(x, finalY - 1));
                    eraseTile(new Vector3(x + 1, finalY));
                    eraseTile(new Vector3(x - 1, finalY));
                    eraseTile(new Vector3(x + 1, finalY + 1));
                    eraseTile(new Vector3(x + 1, finalY - 1));
                    eraseTile(new Vector3(x - 1, finalY + 1));
                    eraseTile(new Vector3(x - 1, finalY - 1));
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
    float minFlagDistance = 2f; // Minimum distance from flag

    while (curObstacles < minObstacles)
    {
        float remainingChance = obstacleChance;
        for (int y = sizeV - vertOffset; y > -vertOffset + 1; y--)
        {
            for (int x = -Mathf.FloorToInt(sizeH / 2); x < Mathf.CeilToInt(sizeH / 2); x++)
            {
                Vector3 spawnPosition = new Vector3(x, y + 1, 0);

                if (!checkIsTile(new Vector3Int(x, y, 0)) && Vector2.Distance(spawnPosition, curFlagPos) > minFlagDistance)
                {
                    RaycastHit2D intersecting = Physics2D.CircleCast(new Vector2(x, y), 0.01f, Vector2.zero);
                    if (!intersecting)
                    {
                        int genAnythingRng = Random.Range(0, 101);
                        if (remainingChance >= genAnythingRng)
                        {
                            int rngTier = Random.Range(0, 3);
                            float rng = Random.Range(1, 101);
                            if (rngTier == 0 && easyChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, easyObstacles.Length);
                                Instantiate(easyObstacles[rngEnemy], spawnPosition, quaternion.identity);
                                curObstacles++;
                            }
                            else if (rngTier == 1 && moderateChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, moderateObstacles.Length);
                                Instantiate(moderateObstacles[rngEnemy], spawnPosition, quaternion.identity);
                                curObstacles++;
                            }
                            else if (rngTier == 2 && hardChance >= rng && curObstacles < maxObstacles)
                            {
                                int rngEnemy = Random.Range(0, hardObstacles.Length);
                                Instantiate(hardObstacles[rngEnemy], spawnPosition, quaternion.identity);
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

    StartCoroutine("floorCanvas");
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
