using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Data;
using UnityEditor;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Constant Variables")]
    private const float speedHardLimit = 30f;
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
    private float curStun;
    private float curFreeze;
    private bool isStunned;
    private bool isFrozen;
    public int activeSkin { get; private set; } = 2;

    [Header("Engine Variables")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D collider2d;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] animations;
    [SerializeField] private ParticleSystem stunParticle;
    [SerializeField] private ParticleSystem freezeParticle;
    [SerializeField] private SpriteRenderer freezeOverlay;
    [SerializeField] [Range(0,1)] private float freezeMagnitude;
    [SerializeField] [Range(0,1)] private float freezeOverlayMagnitude;
    [HideInInspector] public bool firstMove = true;

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
        if (inputReader.touchActive && trackStartPos && !isStunned)
        {
            touchStart = currentDir;
            trackTime = true;
            trackStartPos = false;
            //trackEndPos = true;
        }
        //grab touch end position on touch end
        if (!inputReader.touchActive && trackEndPos && !isStunned)
        {
            touchEnd = currentDir;
            trackTime = false;
            trackEndPos = false;
            //trackStartPos = true;
        }

        if (shootBall && !isStunned)
        {
            ShootBall();
            shootBall = false;

            if (firstMove)
            {
                firstMove = false;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("EndDestroy");
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.GetComponent<EnemyController>())
                    {
                        enemy.GetComponent<EnemyController>().canMove = true;
                    }
                }
            }
        }

        animator.speed = playerSpeed.magnitude;

        if (playerSpeed.magnitude > 0.1f)
        {
            rb.MoveRotation(Vector2.SignedAngle(Vector2.up, playerSpeed.normalized));
        }
    }
    
    private void Update()
    {
        //Stun stuff
        if (curStun > 0)
        {
            curStun -= Time.deltaTime;
        } else if (isStunned)
        {
            speedVector = Vector2.zero;
            trackStartPos = true;
            trackEndPos = false;
            trackTime = false;
            shootBall = false;
            timer = 0;
            isStunned = false;
            stunParticle.Stop();
        }
        
        //Frost stuff
        if (curFreeze > 0)
        {
            curFreeze -= Time.deltaTime;
        } else if (isFrozen)
        {
            isFrozen = false;
            freezeParticle.Stop();
        }

        if (isFrozen)
        {
            freezeOverlay.color = new Vector4(freezeOverlay.color.r, freezeOverlay.color.g, freezeOverlay.color.b,Mathf.Lerp(freezeOverlay.color.a, freezeOverlayMagnitude, Time.deltaTime * 2));
        }
        else
        {
            freezeOverlay.color = new Vector4(freezeOverlay.color.r, freezeOverlay.color.g, freezeOverlay.color.b,Mathf.Lerp(freezeOverlay.color.a, 0, Time.deltaTime));
        }
    }

    public void resetConditions()
    {
        curFreeze = 0;
        curStun = 0;
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


        //Debug.Log((1 / reverseStrength) - lowerLimit);

        //return actual strength after corrections
        if (isFrozen)
        {
            return Mathf.Clamp(((1 / reverseStrength) - lowerLimit)*(1-freezeMagnitude), 0f, speedHardLimit);
        }
        return Mathf.Clamp((1 / reverseStrength) - lowerLimit, 0f, speedHardLimit);
    }

    private void ShootBall()
    {
        //calc normalized value to get movement direction
        shootAngle = (touchEnd - touchStart).normalized;

        //calc force to add by multiplying angle with reversed duration of touch
        Vector2 forceToAdd = shootAngle * CalcStrength();
        //Debug.Log(message: $"shoot angle: {shootAngle} force: {forceToAdd}");

        speedVector = forceToAdd;

        //this prevents misinput at start of game
        if (forceToAdd != null)
        {
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    public void stunPlayer(float duration, bool canOverwrite)
    {
        if (canOverwrite && curStun < duration)
        {
            curStun = duration;
            isStunned = true;
            stunParticle.Play();
        }
        else if (curStun <= 0)
        {
            curStun = duration;
            isStunned = true;
            stunParticle.Play();
        }
    }
    
    public void freezePlayer(float duration, bool canOverwrite)
    {
        if (canOverwrite && curStun < duration)
        {
            curFreeze = duration;
            isFrozen = true;
            freezeParticle.Play();
        }
        else if (curStun <= 0)
        {
            curFreeze = duration;
            isFrozen = true;
            freezeParticle.Play();
        }
    }

    public void ChangeSkin(int _newSkinNumber)
    {
        animator.Play(animations[_newSkinNumber].name);
        activeSkin = _newSkinNumber;
    }

    public void LoadData(GameData data)
    {
        this.activeSkin = data.activeSkin;
        animator.Play(animations[activeSkin].name);
    }

    public void SaveData(ref GameData data)
    {
        data.activeSkin = this.activeSkin;
    }

    #endregion

    #region gizmos

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + (0.2f * speedVector));
    }

    #endregion
}
