using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private Color equippedColor = new Color(0.41568627451f, 0.76470588235f , 0.7294117647f, 0.5f);
    private Color disabledColor = new Color(0.78431372549f, 0.78431372549f, 0.78431372549f, 0.5f);
    private bool[] skinsOwned;
    private int skinsAmount;
    [SerializeField] private TextMeshProUGUI shardsText;

    void Awake()
    {
        skinsOwned = new bool[skins.Length];
        for (int i = 0; i < skinsOwned.Length; i++)
        {
            skinsOwned[i] = false;
            if (playerController.activeSkin == i)
            {
                skinsOwned[i] = true;
            }
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
                lockerButton.interactable = false;
                addToLocker.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (skinsOwned[i] && playerController.activeSkin == i)
            {
                lockerButton.interactable = false;
                var newColorBlock = lockerButton.colors;
                newColorBlock.disabledColor = equippedColor;
                lockerButton.colors = newColorBlock;
                addToLocker.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                lockerButton.interactable = true;
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
        UpdateShards();

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            GameObject addToShop = Instantiate(shopObject, shopItemHolder.transform);
            addToShop.GetComponent<Image>().sprite = skins[i];
            //add listener to menu button component: on click, call ChangeSkin from PlayerController
            //meaning skin Sprites should be in same order on this script as AnimationClips on PlayerController
            Button shopButton = addToShop.GetComponent<Button>();
            TextMeshProUGUI buttonTxt = shopButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            GameObject ownedCover = addToShop.transform.GetChild(0).gameObject;
            shopButton.onClick.AddListener(() => BoughtItem(saveIndex, skinPrices[saveIndex], ownedCover));
            if (skinsOwned[i])
            {
                shopButton.interactable = false;
                ownedCover.SetActive(true);
                var newColorBlock = shopButton.colors;
                newColorBlock.disabledColor = equippedColor;
                shopButton.colors = newColorBlock;
                shopButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (skinPrices[i] > GameManager.Management.shards)
            {
                shopButton.interactable = false;
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skinPrices[saveIndex].ToString();
                buttonTxt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonTxt.text;
            }
            else
            {
                shopButton.interactable = true;
                ownedCover.SetActive(false);
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skinPrices[saveIndex].ToString();
                buttonTxt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonTxt.text;
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
            Button thisButton = lockerItem.GetComponent<Button>();
            if (i != _equippedIndex && skinsOwned[i])
            {
                //ugly ass way to do this but honestly i ran out of good ideas
                lockerItem.transform.GetChild(0).gameObject.SetActive(false);
                lockerItem.transform.GetChild(1).gameObject.SetActive(false);
                thisButton.interactable = true;
                var newColorBlock = thisButton.colors;
                newColorBlock.disabledColor = disabledColor;
                thisButton.colors = newColorBlock;
            }
            else if (i == _equippedIndex)
            {
                lockerItem.transform.GetChild(1).gameObject.SetActive(true);
                var newColorBlock = thisButton.colors;
                newColorBlock.disabledColor = equippedColor;
                thisButton.colors = newColorBlock;
                thisButton.interactable = false;
            }
            else
            {
                lockerItem.transform.GetChild(0).gameObject.SetActive(true);
                lockerItem.transform.GetChild(1).gameObject.SetActive(false);
                thisButton.interactable = false;
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
            UpdateShards();
        }

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            Button shopButton = shopItemHolder.transform.GetChild(i).GetComponent<Button>();
            TextMeshProUGUI buttonTxt = shopButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            GameObject ownedCover = shopButton.transform.GetChild(0).gameObject;
            if (skinsOwned[i])
            {
                shopButton.interactable = false;
                ownedCover.SetActive(true);
                var newColorBlock = shopButton.colors;
                newColorBlock.disabledColor = equippedColor;
                shopButton.colors = newColorBlock;
                shopButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (skinPrices[i] > GameManager.Management.shards)
            {
                shopButton.interactable = false;
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skinPrices[saveIndex].ToString();
                buttonTxt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonTxt.text;
            }
            else
            {
                shopButton.interactable = true;
                ownedCover.SetActive(false);
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skinPrices[saveIndex].ToString();
                buttonTxt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonTxt.text;
            }
        }
    }

    private void UpdateShards()
    {
        shardsText.text = GameManager.Management.shards.ToString();
    }

    public void LoadData(GameData data)
    {
        if (data.skinsOwned == null || data.skinsOwned.Count == 0 )
        {
            Debug.Log("no data found by menumanager");
            return;
        }
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
