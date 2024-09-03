using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walker : MonoBehaviour
{
    [SerializeField] private float upChance, sideChance, downChance;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        startFiller.filler.eraseTile(transform.position);
        Move();
    }

    private void Move()
    {
        int rng = Random.Range(0, 3); //0 up 1 left 2 right 3 down
        int actualRng = Random.Range(0, 100);

        if (rng == 0 && actualRng <= upChance)
        {
            transform.position += new Vector3(0, 1, 0);
        }
        else if (rng == 1 && actualRng <= sideChance)
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        else if (rng == 2 && actualRng <= sideChance)
        {
            transform.position += new Vector3(1, 0, 0);
        }
        else if (rng == 0 && actualRng <= downChance)
        {
            transform.position += new Vector3(0, -1, 0);
        }
    }
}
