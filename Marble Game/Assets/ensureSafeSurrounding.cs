using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ensureSafeSurrounding : MonoBehaviour
{
    [SerializeField] private int radius;
    private void Awake()
    {
        startFiller mapGen = startFiller.filler;
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                mapGen.eraseTile(transform.position + new Vector3(x,y,0));
            }
        }
    }
}
