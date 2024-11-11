using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class advertController : MonoBehaviour
{
    private int currentAd;
    [SerializeField] private Sprite[] visuals;
    [SerializeField] private string[] links;
    [SerializeField] private GameObject advert;
    private Image advertImage;

    private void Start()
    {
        advertImage = advert.GetComponent<Image>();
    }

    public void clickedAd()
    {
        if (links[currentAd] != null || links[currentAd] != "")
        {
            Application.OpenURL(links[0]);
            Debug.Log("Should open: " + links[currentAd].ToString());
        }
    }

    public void closeAd()
    {
        advert.SetActive(false);
    }
    
    public void startAdvert()
    {
        currentAd = Random.Range(0, links.Length);
        advertImage.sprite = visuals[currentAd];
        advert.SetActive(true);
    }
}
