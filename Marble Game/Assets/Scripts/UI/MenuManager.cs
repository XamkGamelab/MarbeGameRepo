using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IDataPersistence
{
    [Header("Const Values")]
    private const float widthForItem = 240;
    private const int tutSeenExpiry = 30;    //this is days

    [Header("UI")]
    [SerializeField] private loadManager loadingManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject lockerMenu;
    [SerializeField] private GameObject lockerHolder;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject shopHolder;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject deleteConfirm;
    [SerializeField] private GameObject deletedPopup;
    [SerializeField] private GameObject menuButtonHolder;
    [SerializeField] private GameObject commonItemsShop;
    [SerializeField] private GameObject rareItemsShop;
    [SerializeField] private GameObject epicItemsShop;
    [SerializeField] private GameObject miscItemsShop;
    [SerializeField] private GameObject commonItemsLocker;
    [SerializeField] private GameObject rareItemsLocker;
    [SerializeField] private GameObject epicItemsLocker;
    [SerializeField] private GameObject miscItemsLocker;
    [SerializeField] private GameObject lockerObject;
    [SerializeField] private GameObject shopObject;
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject enemyTutorialObject;
    [SerializeField] private TextMeshProUGUI enemyTutorialText;
    [SerializeField] private Skin[] skins;
    [SerializeField] private TextMeshProUGUI shardsText;

    [Header("UI Values")]
    private Color equippedColor = new Color(0.41568627451f, 0.76470588235f, 0.7294117647f, 0.5f);
    private Color disabledColor = new Color(0.78431372549f, 0.78431372549f, 0.78431372549f, 0.5f);
    private int skinsAmount;
    public bool tutSeen { get; private set; }
    public bool enemyTutSeen { get; private set; }
    private bool tutFadeRunning = false;
    [SerializeField] private float tutFadeInTime = 1f;
    private Color tutTextColor;
    private Color tutPanelColor;
    public DateTime lastOpenedTime;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer volumeMaster;

    [Header("Singleton")]
    public static MenuManager instance;

    void Awake()
    {
        InitSkinValues();

        if (instance != null && instance != this)
        {
            Debug.LogError("Found more than one MenuManager");
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SavedVolume", 100));
        tutTextColor = tutorialText.color;
        tutPanelColor = tutorialObject.GetComponent<Image>().color;
        tutorialText.color = new Color(tutTextColor.r, tutTextColor.g, tutTextColor.b, 0);
        tutorialObject.GetComponent<Image>().color = new Color(tutPanelColor.r, tutPanelColor.g, tutPanelColor.b, 0);
    }

    private void InitSkinValues()
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

    public void OpenLocker()
    {
        RectTransform vertRect = lockerHolder.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(vertRect);

        audioManager.Management.PlaySimpleClip("Click");
        lockerMenu.SetActive(true);
        menuButtonHolder.SetActive(false);

        float width1 = 0;
        float width2 = 0;
        float width3 = 0;
        float width4 = 0;
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
            else if (skins[i].rarity == Skin.Rarity.Epic)
            {
                addToLocker = Instantiate(lockerObject, epicItemsLocker.transform);
                width3 += widthForItem;
            }
            else
            {
                addToLocker = Instantiate(lockerObject, miscItemsLocker.transform);
                width4 += widthForItem;
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
        
        commonItemsLocker.GetComponent<RectTransform>().sizeDelta = new(width1, objHeight);
        rareItemsLocker.GetComponent<RectTransform>().sizeDelta = new(width2, objHeight);
        epicItemsLocker.GetComponent<RectTransform>().sizeDelta = new(width3, objHeight);
        miscItemsLocker.GetComponent<RectTransform>().sizeDelta = new(width4, objHeight);

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
        foreach (Transform child in epicItemsLocker.transform)
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
        RectTransform vertRect = lockerHolder.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(vertRect);
        
        audioManager.Management.PlaySimpleClip("Click");
        shopMenu.SetActive(true);
        menuButtonHolder.SetActive(false);
        UpdateShards();

        float width1 = 0;
        float width2 = 0;
        float width3 = 0;
        float width4 = 0;
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
            else if (skins[i].rarity == Skin.Rarity.Epic)
            {
                addToShop = Instantiate(shopObject, epicItemsShop.transform);
                width3 += widthForItem;
            }
            else
            {
                addToShop = Instantiate(shopObject, miscItemsShop.transform);
                width4 += widthForItem;
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

        commonItemsShop.GetComponent<RectTransform>().sizeDelta = new(width1, objHeight);
        rareItemsShop.GetComponent<RectTransform>().sizeDelta = new(width2, objHeight);
        epicItemsShop.GetComponent<RectTransform>().sizeDelta = new(width3, objHeight);
        miscItemsShop.GetComponent<RectTransform>().sizeDelta = new(width4, objHeight);
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
        foreach (Transform child in epicItemsShop.transform)
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
        loadingManager.dataDeletion();
        DataPersistenceManager.instance.dataHandler.DeleteData();
        DataPersistenceManager.instance.LoadGame();
        InitSkinValues();
        deletedPopup.SetActive(true);
        deleteConfirm.SetActive(false);
        startFiller.filler.mapGeneration();
    }

    public void CloseDeletePopup()
    {
        audioManager.Management.PlaySimpleClip("Click");
        deletedPopup.SetActive(false);
    }

    public void FadeInTutorial()
    {
        bool sceneHasEnemies = FindObjectsOfType<EnemyController>().Length > 0;

        if (!tutSeen && !tutFadeRunning)
        {
            StartCoroutine(TutorialFader());
        }
        else if (tutSeen && !enemyTutSeen && sceneHasEnemies)
        {
            StartCoroutine(EnemyTutorialFader());
        }
    }

    private IEnumerator TutorialFader()
    {
        tutFadeRunning = true;
        tutorialObject.SetActive(true);

        float takenTime = 0;
        float tutTextAlpha = 0;
        float tutPanelAlpha = 0;

        while (takenTime < tutFadeInTime)
        {
            takenTime += Time.deltaTime;

            tutTextAlpha = Mathf.Lerp(0, tutTextColor.a, takenTime / tutFadeInTime);
            tutPanelAlpha = Mathf.Lerp(0, tutPanelColor.a, takenTime / tutFadeInTime);

            Color newTextColor = new(tutTextColor.r , tutTextColor.g, tutTextColor.b, tutTextAlpha);
            Color newPanelColor = new(tutPanelColor.r, tutPanelColor.g, tutPanelColor.b, tutPanelAlpha);

            tutorialObject.GetComponent<Image>().color = newPanelColor;
            tutorialText.color = newTextColor;

            yield return null;
        }
        tutSeen = true;
        tutFadeRunning = false;
    }

    private IEnumerator EnemyTutorialFader()
    {
        playerController.trackMoves = true;
        tutFadeRunning = true;
        enemyTutorialObject.SetActive(true);

        float takenTime = 0;
        float tutTextAlpha = 0;
        float tutPanelAlpha = 0;

        while (takenTime < tutFadeInTime)
        {
            takenTime += Time.deltaTime;

            tutTextAlpha = Mathf.Lerp(0, tutTextColor.a, takenTime / tutFadeInTime);
            tutPanelAlpha = Mathf.Lerp(0, tutPanelColor.a, takenTime / tutFadeInTime);

            Color newTextColor = new(tutTextColor.r, tutTextColor.g, tutTextColor.b, tutTextAlpha);
            Color newPanelColor = new(tutPanelColor.r, tutPanelColor.g, tutPanelColor.b, tutPanelAlpha);

            enemyTutorialObject.GetComponent<Image>().color = newPanelColor;
            enemyTutorialText.color = newTextColor;

            yield return null;
        }
        tutSeen = true;
        tutFadeRunning = false;
    }

    public void CloseTutorial()
    {
        tutorialObject.SetActive(false);
        tutSeen = true;
    }
    public void CloseEnemyTutorial()
    {
        enemyTutorialObject.SetActive(false);
        enemyTutSeen = true;
    }

    private void EquippedItem(int _equippedIndex)
    {
        audioManager.Management.PlaySimpleClip("Click");
        if (!skins[_equippedIndex].owned) return;
        playerController.ChangeSkin(_equippedIndex);

        int commonCounter = 0;
        int rareCounter = 0;
        int epicCounter = 0;
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
            else if (skins[i].rarity == Skin.Rarity.Epic)
            {
                lockerItem = epicItemsLocker.transform.GetChild(epicCounter).gameObject;
                epicCounter++;
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
        int epicCounter = 0;
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
            else if (skins[i].rarity == Skin.Rarity.Epic)
            {
                shopButton = epicItemsShop.transform.GetChild(epicCounter).GetComponent<Button>();
                epicCounter++;
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

        for (int i = 0; i < this.skins.Count(); i++)
        {
            if (i < data.skinOwned.Count)
            {
                skins[i].owned = data.skinOwned[i];
            }
        }

        //load tutorial seen variables, but reset them if game was last opened less than tutSeenExpiry days ago
        this.enemyTutSeen = false;
        if (data.tutSeen && (DateTime.Now - data.lastOpenedTime).TotalDays > tutSeenExpiry)
        {
            this.tutSeen = false;
        }
        else
        {
            this.tutSeen = data.tutSeen;
            this.enemyTutSeen = data.enemyTutSeen;
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

        data.tutSeen = this.tutSeen;
        data.enemyTutSeen = this.enemyTutSeen;

        data.lastOpenedTime = DateTime.Now;
    }
}