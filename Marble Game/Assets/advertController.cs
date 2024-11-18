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
    [SerializeField] private GameObject advertHolder;
    private Image advertImage;

    private void Start()
    {
        advertImage = advert.GetComponent<Image>();
    }

    public void clickedAd()
    {
        if (links[currentAd] != null || links[currentAd] != "")
        {
            Application.OpenURL(links[currentAd]);
            Debug.Log("Should open: " + links[currentAd].ToString());
        }
    }

    public void closeAd()
    {
        audioManager.Management.PlaySimpleClip("Click");
        advertHolder.SetActive(false);
    }
    
    public void startAdvert()
    {
        advertHolder.SetActive(true);
        currentAd = Random.Range(0, links.Length);
        Debug.Log(currentAd);
        advertImage.sprite = visuals[currentAd];
    }
}
