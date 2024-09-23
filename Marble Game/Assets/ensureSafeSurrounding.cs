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
        
        
        //Guarantee it works
        /*
        if (radius == 1)
        {
            mapGen.eraseTile(transform.position + new Vector3(1,0,0));
            mapGen.eraseTile(transform.position + new Vector3(1,1,0));
            mapGen.eraseTile(transform.position + new Vector3(-1,0,0));
            mapGen.eraseTile(transform.position + new Vector3(-1,1,0));
            mapGen.eraseTile(transform.position + new Vector3(0,1,0));
            mapGen.eraseTile(transform.position + new Vector3(0,-1,0));
            mapGen.eraseTile(transform.position + new Vector3(-1,-1,0));
            mapGen.eraseTile(transform.position + new Vector3(1,-1,0));
        }
        */
        
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                mapGen.eraseTile(transform.position + new Vector3(x,y,0));
            }
        }
    }
}
