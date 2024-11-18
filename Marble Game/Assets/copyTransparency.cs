using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class copyTransparency : MonoBehaviour
{
    [SerializeField] private GameObject objToMimic;
    private Image mimicedImg;
    private Image thisImg;

    private void Start()
    {
        thisImg = gameObject.GetComponent<Image>();
        mimicedImg = objToMimic.GetComponent<Image>();
    }

    private void Update()
    {
        thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b,
            mimicedImg.color.a);
    }
}
