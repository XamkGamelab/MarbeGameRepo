using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject lockerMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject menuButtonHolder;
    [SerializeField] private GameObject lockerItemHolder;
    [SerializeField] private GameObject shopItemHolder;
    [SerializeField] private GameObject lockerObject;
    [SerializeField] private GameObject shopObject;
    [SerializeField] private Sprite[] skins;
    [Range(1, 50)][SerializeField] int[] skinPrices;
    private bool[] skinsOwned;
    private int skinsAmount;

    void Awake()
    {
        skinsOwned = new bool[skins.Length];
        for (int i = 0; i < skinsOwned.Length; i++)
        {
            skinsOwned[i] = false;
        }
        skinsAmount = skinsOwned.Length;
    }

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

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            //instantiate menu buttons, change their sprites to match
            GameObject addToLocker = Instantiate(lockerObject, lockerItemHolder.transform);
            addToLocker.GetComponent<Image>().sprite = skins[i];
            //add listener to mennu button component: on click, call ChangeSkin from PlayerController
            //meaning skin Sprites should be in same order on this script as AnimationClips on PlayerController
            Button lockerButton = addToLocker.GetComponent<Button>();
            lockerButton.onClick.AddListener(() => EquippedItem(saveIndex));
            if (!skinsOwned[i])
            {
                lockerButton.enabled = false;
                addToLocker.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (skinsOwned[i] && playerController.activeSkin == i)
            {
                lockerButton.enabled = false;
                addToLocker.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                lockerButton.enabled = true;
            }
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
        DataPersistenceManager.instance.SaveGame();
    }

    public void OpenShop()
    {
        shopMenu.SetActive(true);
        menuButtonHolder.SetActive(false);

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            GameObject addToShop = Instantiate(shopObject, shopItemHolder.transform);
            addToShop.GetComponent<Image>().sprite = skins[i];
            //add listener to mennu button component: on click, call ChangeSkin from PlayerController
            //meaning skin Sprites should be in same order on this script as AnimationClips on PlayerController
            Button shopButton = addToShop.GetComponent<Button>();
            GameObject ownedCover = addToShop.transform.GetChild(0).gameObject;
            shopButton.onClick.AddListener(() => BoughtItem(saveIndex, skinPrices[saveIndex], ownedCover));
            if (!skinsOwned[i])
            {
                shopButton.enabled = true;
                ownedCover.SetActive(false);
            }
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
        DataPersistenceManager.instance.SaveGame();
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

    private void EquippedItem(int _equippedIndex)
    {
        if (!skinsOwned[_equippedIndex]) return;
        playerController.ChangeSkin(_equippedIndex);
        for (int i = 0; i < skinsAmount; i++)
        {
            GameObject lockerItem = lockerItemHolder.transform.GetChild(i).gameObject;
            if (i != _equippedIndex && skinsOwned[i])
            {
                //ugly ass way to do this but honestly i ran out of good ideas
                lockerItem.transform.GetChild(0).gameObject.SetActive(false);
                lockerItem.transform.GetChild(1).gameObject.SetActive(false);
                lockerItem.transform.gameObject.GetComponent<Button>().enabled = true;
            }
            else if (i == _equippedIndex)
            {
                lockerItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                lockerItem.transform.GetChild(0).gameObject.SetActive(true);
                lockerItem.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void BoughtItem(int _buttonNumber, int _skinPrice, GameObject _ownedCover)
    {
        if (GameManager.Management.shards >= _skinPrice)
        {
            GameManager.Management.shards -= _skinPrice;
            skinsOwned[_buttonNumber] = true;
            _ownedCover.SetActive(true);
        }
    }

    public void LoadData(GameData data)
    {
        if (data.skinsOwned.Count == 0 || data.skinsOwned == null) return;
        if (this.skinsAmount != data.skinsAmount)
        {
            List<bool> currentSkins = data.skinsOwned.ToList();
            while (currentSkins.Count < data.skinsOwned.Count)
            {
                currentSkins.Insert(0, false);
            }
            this.skinsOwned = currentSkins.ToArray();
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
