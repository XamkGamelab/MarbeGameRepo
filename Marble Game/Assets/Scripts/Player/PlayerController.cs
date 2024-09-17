using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Data;

public class PlayerController : MonoBehaviour
{
    [Header("Constant Variables")]
    private const float powerLimit = 2f;
    private const float lowerLimit = 0.1f;

    [Header("Dynamic Variables")]
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private float shootStrength;
    private Vector2 shootAngle = Vector2.zero;
    private Vector2 currentDir = Vector2.zero;
    //vector below is for gizmo usage
    private Vector2 speedVector = Vector2.zero;
    private bool trackStartPos = true;
    private bool trackEndPos = false;
    private bool trackTime = false;
    private bool shootBall = false;
    private float timer = 0;

    [Header("Engine Variables")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D collider2d;
    [SerializeField] private Animator animator;

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
        Vector2 playerSpeed = rb.velocity;

        //timer is run when screen has touch
        if (trackTime) Timer();
        else if (!trackTime) timer = 0;

        //grab touch starting position on touch start
        if (inputReader.touchActive && trackStartPos)
        {
            touchStart = currentDir;
            trackTime = true;
            trackStartPos = false;
            //trackEndPos = true;
        }
        //grab touch end position on touch end
        if (!inputReader.touchActive && trackEndPos)
        {
            touchEnd = currentDir;
            trackTime = false;
            trackEndPos = false;
            //trackStartPos = true;
        }

        if (shootBall)
        {
            ShootBall();
            shootBall = false;
        }

        animator.speed = playerSpeed.magnitude;

        if (playerSpeed.magnitude > 0.1f)
        {
            rb.MoveRotation(Vector2.SignedAngle(Vector2.up, playerSpeed.normalized));
        }
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
        trackStartPos = true;
    }

    //called on touch end
    private void HandleTouchEnd()
    {
        trackEndPos = true;
        shootBall = true;
    }

    private void CalcDirection(Vector2 dir)
    {
        //save dir to separate vector to use in FixedUpdate
        currentDir = dir;
    }

    private float CalcStrength()
    {
        //normalize value to be between 0 and 1
        float reverseStrength = timer / powerLimit;


        Debug.Log((1 / reverseStrength) - lowerLimit);

        //return actual strength after corrections
        return (1 / reverseStrength) - lowerLimit;
    }

    private void ShootBall()
    {
        //calc normalized value to get movement direction
        shootAngle = (touchEnd - touchStart).normalized;

        //calc force to add by multiplying angle with reversed duration of touch
        Vector2 forceToAdd = shootAngle * CalcStrength();
        Debug.Log(message: $"shoot angle: {shootAngle} force: {forceToAdd}");

        speedVector = forceToAdd;

        //this prevents misinput at start of game
        if (forceToAdd != null)
        {
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region gizmos

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(gameObject.transform.position, speedVector);
    }

    #endregion
}
