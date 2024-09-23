using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class walker : MonoBehaviour
{
    public float upChance, sideChance, downChance;
    public float deathChance = 100f;
    public int minMoves;
    public float newWalkerSpawn = 0f;

    [SerializeField] private GameObject newWalker;
    private int bounds;
    
    private Vector3Int intPos;
    [SerializeField] private float moveDelay;
    private float curDelay;

    private void Start()
    {
        bounds = startFiller.filler.walkerBounds;
    }

    void Update()
    {
        intPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
        
        if (moveDelay > curDelay)
        {
            curDelay += Time.deltaTime;
        }
        else
        {
            curDelay = 0;
            Move();
        }
    }

    private void Move()
    {
        startFiller.filler.eraseTile(transform.position);
        
        int rng = Random.Range(0, 4); //0 up 1 left 2 right 3 down
        int actualRng = Random.Range(1, 101);

        if (rng == 0 && actualRng <= upChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(0, bounds+10, 0)))
            {
                transform.position += new Vector3(0, 1, 0);
                minMoves--;
            }
            else
            {
                if (downChance < upChance)
                {
                    downChance = upChance;
                }
                upChance /= 2;
            }
        }
        else if (rng == 1 && actualRng <= sideChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(-bounds, 0, 0)))
            {
                transform.position += new Vector3(-1, 0, 0);
                minMoves--;
            }
        }
        else if (rng == 2 && actualRng <= sideChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(bounds, 0, 0)))
            {
                transform.position += new Vector3(1, 0, 0);
                minMoves--;
            }
        }
        else if (rng == 3 && actualRng <= downChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(0, -bounds, 0)))
            {
                transform.position += new Vector3(0, -1, 0);
                minMoves--;
            }
        }
        float newSpawnRng = Random.Range(0, 101);
        if (newSpawnRng <= newWalkerSpawn)
        {
            newWalkerSpawn *= 0.75f;
            startFiller.filler.remainingWalkers++;
            GameObject childWalker = Instantiate(newWalker, transform.position, quaternion.identity);
        }

        float deathRng = Random.Range(0, 101);
        if (minMoves <= 0 && deathRng <= deathChance)
        {
            startFiller.filler.remainingWalkers--;
            Destroy(gameObject);
        }
    }
}
