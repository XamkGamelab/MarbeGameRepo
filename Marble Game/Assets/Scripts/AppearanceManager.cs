using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AppearanceManager : MonoBehaviour
{
    [Header("Engine Variables")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AnimationClip[] skins;
    private int playerSkinNumber = 0;

    //temp vars
    private bool swapSkin = true;

    //TODO
    //TODO
    //TODO
    //TODO
    //TODO
    //Add method to change player sprite via separate "locker"-menu buttons
    //Each button should update playerSkinNumber and tell animator to play correct clip

    void Awake()
    {
        playerAnimator.Play(skins[playerSkinNumber].name);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && swapSkin == true)
        {
            playerAnimator.Play(skins[playerSkinNumber].name);
            swapSkin = false;
        }
    }
}
