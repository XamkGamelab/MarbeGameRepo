using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class walker : MonoBehaviour
{
    public float upChance, sideChance, downChance;
    public float deathChance = 100f;
    public int minMoves;
    
    private Vector3Int intPos;
    [SerializeField] private float moveDelay;
    private float curDelay;

    // Update is called once per frame
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
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(0, 2, 0)))
            {
                transform.position += new Vector3(0, 1, 0);
                minMoves--;
            }
        }
        else if (rng == 1 && actualRng <= sideChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(-2, 0, 0)))
            {
                transform.position += new Vector3(-1, 0, 0);
                minMoves--;
            }
        }
        else if (rng == 2 && actualRng <= sideChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(2, 0, 0)))
            {
                transform.position += new Vector3(1, 0, 0);
                minMoves--;
            }
        }
        else if (rng == 3 && actualRng <= downChance)
        {
            if (startFiller.filler.checkBounds(intPos + new Vector3Int(0, -2, 0)))
            {
                transform.position += new Vector3(0, -1, 0);
                minMoves--;
            }
        }

        float deathRng = Random.Range(1, 101);
        if (minMoves <= 0 && deathRng <= deathChance)
        {
            Destroy(gameObject);
        }
    }
}
