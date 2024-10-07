using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, InputManager.IGameplayActions
{
    private InputManager inputActions;
    public event Action<Vector2> MoveEvent;
    public event Action TouchEvent;
    public event Action TouchCanceledEvent;
    public bool touchActive { get; private set; } = false;

    private void OnEnable()
    {
        //instance an InputManager and activate it
        if (inputActions == null)
        {
            inputActions = new InputManager();
            inputActions.Gameplay.SetCallbacks(this);

            SetGameplay();
        }
    }

    public void SetGameplay()
    {
        inputActions.Gameplay.Enable();
    }

    public void DisableGameplay()
    {

        inputActions.Gameplay.Disable();
    }

    //this is called whenever pointer or touch moves, vector2 given as context
    public void OnMovement(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(obj: context.ReadValue<Vector2>());
    }

    //this is called whenever touch starts, changes or happens, no context needed
    public void OnIsTouching(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            touchActive = true;
            TouchEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            touchActive = false;
            TouchCanceledEvent?.Invoke();
        }

    }
}
