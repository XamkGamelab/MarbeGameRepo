using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IDataPersistence
{
    [Header("Const Values")]
    private const float widthForItem = 240;

    [Header("UI")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject lockerMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject deleteConfirm;
    [SerializeField] private GameObject deletedPopup;
    [SerializeField] private GameObject menuButtonHolder;
    [SerializeField] private GameObject commonItemsShop;
    [SerializeField] private GameObject rareItemsShop;
    [SerializeField] private GameObject miscItemsShop;
    [SerializeField] private GameObject commonItemsLocker;
    [SerializeField] private GameObject rareItemsLocker;
    [SerializeField] private GameObject miscItemsLocker;
    [SerializeField] private GameObject lockerObject;
    [SerializeField] private GameObject shopObject;
    [SerializeField] private Skin[] skins;
    private Color equippedColor = new Color(0.41568627451f, 0.76470588235f , 0.7294117647f, 0.5f);
    private Color disabledColor = new Color(0.78431372549f, 0.78431372549f, 0.78431372549f, 0.5f);
    private int skinsAmount;
    [SerializeField] private TextMeshProUGUI shardsText;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer volumeMaster;

    void Awake()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].owned = false;
            if (playerController.activeSkin == i)
            {
                skins[i].owned = true;
            }
        }
        skinsAmount = skins.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedVolume", 100));
    }

    public void OpenLocker()
    {
        audioManager.Management.PlaySimpleClip("Click");
        lockerMenu.SetActive(true);
        menuButtonHolder.SetActive(false);

        float width1 = 0;
        float width2 = 0;
        float width3 = 0;
        float objHeight = commonItemsLocker.GetComponent<RectTransform>().rect.height;


        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            GameObject addToLocker;

            //common skins to common skin holder et cetera
            if (skins[i].rarity == Skin.Rarity.Common)
            {
                addToLocker = Instantiate(lockerObject, commonItemsLocker.transform);
                width1 += widthForItem;
            }
            else if (skins[i].rarity == Skin.Rarity.Rare)
            {
                addToLocker = Instantiate(lockerObject, rareItemsLocker.transform);
                width2 += widthForItem;
            }
            else
            {
                addToLocker = Instantiate(lockerObject, miscItemsLocker.transform);
                width3 += widthForItem;
            }

            addToLocker.GetComponent<Image>().sprite = skins[i].sprite;
            //add listener to menu button component: on click, call ChangeSkin from PlayerController
            //meaning skin Sprites should be in same order on this script as AnimationClips on PlayerController
            Button lockerButton = addToLocker.GetComponent<Button>();
            lockerButton.onClick.AddListener(() => EquippedItem(saveIndex));
            if (!skins[i].owned)
            {
                lockerButton.interactable = false;
                addToLocker.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (skins[i].owned && playerController.activeSkin == i)
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
        
        commonItemsLocker.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width1 ,objHeight);
        rareItemsLocker.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width2 ,objHeight);
        miscItemsLocker.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width3 ,objHeight);
    }

    public void CloseLocker()
    {
        audioManager.Management.PlaySimpleClip("Click");
        lockerMenu.SetActive(false);
        menuButtonHolder.SetActive(true);

        foreach (Transform child in commonItemsLocker.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in rareItemsLocker.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in miscItemsLocker.transform)
        {
            Destroy(child.gameObject);
        }

        DataPersistenceManager.instance.SaveGame();
    }

    public void OpenShop()
    {
        audioManager.Management.PlaySimpleClip("Click");
        shopMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
        UpdateShards();

        float width1 = 0;
        float width2 = 0;
        float width3 = 0;
        float objHeight = commonItemsShop.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            GameObject addToShop;

            //common skins to common skin holder et cetera
            if (skins[i].rarity == Skin.Rarity.Common)
            {
                addToShop = Instantiate(shopObject, commonItemsShop.transform);
                width1 += widthForItem;
            }
            else if (skins[i].rarity == Skin.Rarity.Rare)
            {
                addToShop = Instantiate(shopObject, rareItemsShop.transform);
                width2 += widthForItem;
            }
            else
            {
                addToShop = Instantiate(shopObject, miscItemsShop.transform);
                width3 += widthForItem;
            }
            
            addToShop.GetComponent<Image>().sprite = skins[saveIndex].sprite;
            
            //add listener to menu button component: on click, call ChangeSkin from PlayerController
            //meaning skin Sprites should be in same order on this script as AnimationClips on PlayerController
            Button shopButton = addToShop.GetComponent<Button>();
            TextMeshProUGUI buttonTxt = shopButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            GameObject ownedCover = addToShop.transform.GetChild(0).gameObject;
            shopButton.onClick.AddListener(() => BoughtItem(saveIndex, skins[saveIndex].price, ownedCover));
            if (skins[i].owned)
            {
                shopButton.interactable = false;
                ownedCover.SetActive(true);
                var newColorBlock = shopButton.colors;
                newColorBlock.disabledColor = equippedColor;
                shopButton.colors = newColorBlock;
                shopButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (skins[i].price > GameManager.Management.shards)
            {
                shopButton.interactable = false;
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skins[saveIndex].price.ToString();
            }
            else
            {
                shopButton.interactable = true;
                ownedCover.SetActive(false);
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skins[saveIndex].price.ToString();
            }
        }

        commonItemsShop.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width1 ,objHeight);
        rareItemsShop.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width2 ,objHeight);
        miscItemsShop.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width3 ,objHeight);
    }

    public void CloseShop()
    {
        audioManager.Management.PlaySimpleClip("Click");
        shopMenu.SetActive(false);
        menuButtonHolder.SetActive(true);

        foreach (Transform child in commonItemsShop.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in rareItemsShop.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in miscItemsShop.transform)
        {
            Destroy(child.gameObject);
        }
        
        DataPersistenceManager.instance.SaveGame();
    }

    public void OpenSettings()
    {
        audioManager.Management.PlaySimpleClip("Click");
        settingsMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
    }

    public void CloseSettings()
    {
        audioManager.Management.PlaySimpleClip("Click");
        settingsMenu.SetActive(false);
        menuButtonHolder.SetActive(true);
    }

    public void OpenCredits()
    {
        audioManager.Management.PlaySimpleClip("Click");
        creditsMenu.SetActive(true);
    }

    public void CloseCredits()
    {
        audioManager.Management.PlaySimpleClip("Click");
        creditsMenu.SetActive(false);
    }

    public void OpenConfirmDelete()
    {
        audioManager.Management.PlaySimpleClip("Click");
        deleteConfirm.SetActive(true);
    }

    public void CloseConfirmDelete()
    {
        audioManager.Management.PlaySimpleClip("Click");
        deleteConfirm.SetActive(false);
    }

    public void DeleteData()
    {
        audioManager.Management.PlaySimpleClip("Click");
        DataPersistenceManager.instance.dataHandler.DeleteData();
        DataPersistenceManager.instance.LoadGame();
        deletedPopup.SetActive(true);
        deleteConfirm.SetActive(false);
        startFiller.filler.mapGeneration();
    }

    public void CloseDeletePopup()
    {
        audioManager.Management.PlaySimpleClip("Click");
        deletedPopup.SetActive(false);
    }

    private void EquippedItem(int _equippedIndex)
    {
        Debug.Log($"equipped item with index {_equippedIndex}");
        audioManager.Management.PlaySimpleClip("Click");
        if (!skins[_equippedIndex].owned) return;
        playerController.ChangeSkin(_equippedIndex);

        int commonCounter = 0;
        int rareCounter = 0;
        int miscCounter = 0;

        for (int i = 0; i < skinsAmount; i++)
        {
            GameObject lockerItem;

            if (skins[i].rarity == Skin.Rarity.Common)
            {
                lockerItem = commonItemsLocker.transform.GetChild(commonCounter).gameObject;
                commonCounter++;
            }
            else if (skins[i].rarity == Skin.Rarity.Rare)
            {
                lockerItem = rareItemsLocker.transform.GetChild(rareCounter).gameObject;
                rareCounter++;
            }
            else
            {
                lockerItem = miscItemsLocker.transform.GetChild(miscCounter).gameObject;
                miscCounter++;
            }

            Button thisButton = lockerItem.GetComponent<Button>();
            if (i != _equippedIndex && skins[i].owned)
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
        audioManager.Management.PlaySimpleClip("Click");
        if (GameManager.Management.shards >= _skinPrice)
        {
            GameManager.Management.shards -= _skinPrice;
            skins[_buttonNumber].owned = true;
            _ownedCover.SetActive(true);
            UpdateShards();
        }

        int commonCounter = 0;
        int rareCounter = 0;
        int miscCounter = 0;

        for (int i = 0; i < skins.Length; i++)
        {
            int saveIndex = i;
            Button shopButton;

            if (skins[i].rarity == Skin.Rarity.Common)
            {
                shopButton = commonItemsShop.transform.GetChild(commonCounter).GetComponent<Button>();
                commonCounter++;
            }
            else if (skins[i].rarity == Skin.Rarity.Rare)
            {
                shopButton = rareItemsShop.transform.GetChild(rareCounter).GetComponent<Button>();
                rareCounter++;
            }
            else
            {
                shopButton = miscItemsShop.transform.GetChild(miscCounter).GetComponent<Button>();
                miscCounter++;
            }

            TextMeshProUGUI buttonTxt = shopButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            GameObject ownedCover = shopButton.transform.GetChild(0).gameObject;
            if (skins[i].owned)
            {
                shopButton.interactable = false;
                ownedCover.SetActive(true);
                var newColorBlock = shopButton.colors;
                newColorBlock.disabledColor = equippedColor;
                shopButton.colors = newColorBlock;
                shopButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (skins[i].price > GameManager.Management.shards)
            {
                shopButton.interactable = false;
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skins[saveIndex].price.ToString();
            }
            else
            {
                shopButton.interactable = true;
                ownedCover.SetActive(false);
                shopButton.transform.GetChild(1).gameObject.SetActive(true);
                buttonTxt.text = skins[saveIndex].price.ToString();
            }
        }
    }

    private void UpdateShards()
    {
        shardsText.text = GameManager.Management.shards.ToString();
    }

    private void SetVolume(float _vol)
    {
        if (_vol < 1f)
        {
            _vol = 0.0001f;
        }
        
        RefreshSlider(_vol);
        PlayerPrefs.SetFloat("SavedVolume", _vol);
        volumeMaster.SetFloat("MasterVolume", Mathf.Log10(_vol / 100) * 20f);
    }

    public void SetVolFromSlider()
    {
        SetVolume(volumeSlider.value);
    }

    private void RefreshSlider(float _vol)
    {
        volumeSlider.value = _vol;
    }

    public void LoadData(GameData data)
    {
        if (data.skinOwned == null || data.skinOwned.Count == 0 )
        {
            Debug.Log("no data found by menumanager");
            return;
        }

        Debug.Log("loading only owned skins");
        for (int i = 0; i < this.skins.Count(); i++)
        {
            if (i < data.skinOwned.Count)
            {
                skins[i].owned = data.skinOwned[i];
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.skinOwned = new List<bool>();
        for (int i = 0; i < this.skins.Length; i++)
        {
            data.skinOwned.Add(skins[i].owned);
        }
        data.skinsAmount = this.skinsAmount;
    }
}