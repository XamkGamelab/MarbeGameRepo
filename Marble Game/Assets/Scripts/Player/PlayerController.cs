using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Constant Variables")]
    private const float powerLimit = 2f;
    private const float lowerLimit = 0.1f;

    [Header("Dynamic Variables")]
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private float shootStrength;
    private Vector2 shootAngle;
    private bool trackStartPos = true;
    private bool trackEndPos = false;
    private bool trackTime = false;
    private float timer = 0;

    [Header("Engine Variables")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D collider2d;

    #region standard methods

    private void Awake()
    {
        //add methods to events called on inputs
        inputReader.MoveEvent += CalcDirection;
        inputReader.TouchEvent += HandleTouch;
        inputReader.TouchCanceledEvent += HandleTouchEnd;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(message: $"touchStart = {touchStart}, touchEnd = {touchEnd}");

        //timer is run when screen has touch
        if (trackTime) Timer();
        else if (!trackTime) timer = 0;
    }

    #endregion

    #region other methods

    private void Timer()
    {
        //timer increases with time until powerLimit is reached
        timer += Time.deltaTime;
        if (timer > powerLimit) timer = powerLimit;
    }

    //called on touch start
    private void HandleTouch()
    {
        trackTime = true;
        trackEndPos = true;
    }

    //called on touch end
    private void HandleTouchEnd()
    {
        ShootBall();
        trackTime = false;
        trackStartPos = true;
    }

    private void CalcDirection(Vector2 dir)
    {
        //LOGIC HERE IS BROKEN
        //LOGIC HERE IS BROKEN
        //LOGIC HERE IS BROKEN
        //LOGIC HERE IS BROKEN
        //most likely cause is saving touchStart or touchEnd or both at wrong time

        //grab touch starting position on touch start
        if (inputReader.touchActive && trackStartPos)
        {
            touchStart = dir;
            trackStartPos = false;
        }
        //grab touch end position on touch end
        if (!inputReader.touchActive && trackEndPos)
        {
            touchEnd = dir;
            trackEndPos = false;
        }

        //calc normalized value to get movement direction
        shootAngle = (touchEnd - touchStart).normalized;
    }

    private float CalcStrength()
    {
        //normalize value to be between 0 and 1
        float reverseStrength = timer / powerLimit;

        //return actual strength after corrections
        return (1 / reverseStrength) - lowerLimit;
    }

    private void ShootBall()
    {
        //calc force to add by multiplying angle with reversed duration of touch
        Vector2 forceToAdd = shootAngle * CalcStrength();
        Debug.Log(message: $"shoot angle: {shootAngle} force: {forceToAdd}");

        //this prevents misinput at start of game
        if (forceToAdd != null)
        {
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    #endregion
}
