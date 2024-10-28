using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject lockerMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject menuButtonHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLocker()
    {
        lockerMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
    }

    public void CloseLocker()
    {
        lockerMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
    }
}
