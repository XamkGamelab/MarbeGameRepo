using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject lockerMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject menuButtonHolder;
    [SerializeField] private GameObject lockerItemHolder;
    [SerializeField] private GameObject shopItemHolder;
    [SerializeField] private GameObject lockerObject;
    [SerializeField] private Sprite[] skins;
    private bool[] skinsOwned;
    private int skinsAmount;

    // Start is called before the first frame update
    void Start()
    {
        skinsOwned = new bool[skins.Length];
        for (int i = 0; i < skinsOwned.Length; i++)
        {
            skinsOwned[i] = false;
        }
        skinsAmount = skinsOwned.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLocker()
    {
        lockerMenu.SetActive(true);
        menuButtonHolder.SetActive(false);

        for (int i = 0; i < skins.Length; i++)
        {
            GameObject addToLocker = lockerObject;
            addToLocker.GetComponent<Image>().sprite = skins[i];
            Instantiate(addToLocker, lockerItemHolder.transform);
        }
    }

    public void CloseLocker()
    {
        lockerMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
        for (int i = 0; i < skins.Length; i++)
        {
            Destroy(lockerItemHolder.transform.GetChild(i).gameObject);
        }
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
        menuButtonHolder.SetActive(false);

        for (int i = 0; i < skins.Length; i++)
        {
            GameObject addToShop = lockerObject;
            addToShop.GetComponent<Image>().sprite = skins[i];
            Instantiate(addToShop, shopItemHolder.transform);
        }
    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
        for (int i = 0; i < skins.Length; i++)
        {
            Destroy(shopItemHolder.transform.GetChild(i).gameObject);
        }
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

    public void LoadData(GameData data)
    {
        if (data.skinsOwned.Count > 0 && data.skinsOwned != null)
        {
            if (this.skinsAmount != data.skinsAmount)
            {
                List<bool> currentSkins = data.skinsOwned.ToList();
                {
                    while (currentSkins.Count < data.skinsOwned.Count)
                    {
                        currentSkins.Insert(0, false);
                    }
                }
                this.skinsOwned = currentSkins.ToArray();
            }
        }
        else if (data.skinsOwned != null)
        {
            this.skinsOwned = data.skinsOwned.ToArray();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.skinsOwned = this.skinsOwned.ToList();
        data.skinsAmount = this.skinsAmount;
    }
}
