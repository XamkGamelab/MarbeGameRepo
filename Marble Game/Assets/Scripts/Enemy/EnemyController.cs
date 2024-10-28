using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Engine Variables")]
    //[SerializeField] private GameManager gameManager;
    private Rigidbody2D rb;
    private GameObject player;
    private Animator enemyAnimator;
    [SerializeField] private bool canWander;
    public bool canMove;

    [Header("Dynamic Variables")]
    private Vector3 playerLoc = Vector3.zero;
    private bool seesPlayer = false;
    private bool timing = false;
    private bool delaying = false;
    private float timer = 0f;

    [SerializeField][Range(0f, 25.0f)] private float lowPower = 5f;
    [SerializeField][Range(0f, 50.0f)] private float highPower = 25f;

    //randomize this based on current level -> smaller value = bigger difficulty
    [SerializeField][Range(1.0f, 10.0f)] private float delayBeforeShoot = 3f;

    //if player closer than this to player, enemy starts chasing, adjustable
    [SerializeField][Range(0f, 15.0f)] private float detectDistance = 10f;

    //Coroutines
    private Coroutine checkForPlayer;
    private Coroutine delayLocationTrack;

    private void Awake()
    {
        //fetch variables
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        checkForPlayer = StartCoroutine(CheckForPlayer());
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Vector2 speed = rb.velocity;

            //timer
            if (timing || canWander)
            {
                timer += Time.deltaTime;
            }

            //if timer high enough, shoot
            if (timer > delayBeforeShoot)
            {
                if (seesPlayer)
                {
                    Shoot();
                }
                else
                {
                    Wander();
                }
            }

            //if stopped and bool on, grab location
            if (!delaying)
            {
                playerLoc = player.transform.position;
                delaying = true;
            }

            enemyAnimator.speed = speed.magnitude;

            if (speed.magnitude > 0.1f)
            {
                rb.MoveRotation(Vector2.SignedAngle(Vector2.up, speed.normalized));
            }
        }
    }

    private IEnumerator CheckForPlayer()
    {
        while (!seesPlayer)
        {
            yield return new WaitForSeconds(0.5f);

            if (Vector2.Distance(gameObject.transform.position, player.transform.position) < detectDistance)
            {
                seesPlayer = true;
                timing = true;
                playerLoc = player.transform.position;
                if (canWander)
                {
                    StartCoroutine("TrackPlayer");
                }
            }
        }
    }
    
    private IEnumerator TrackPlayer()
    {
        //Once it sees player, it always sees player.
        while (seesPlayer)
        {
            yield return new WaitForSeconds(0.5f);

            if (Vector2.Distance(gameObject.transform.position, player.transform.position) > detectDistance*2)
            {
                seesPlayer = false;
                timing = false;
                playerLoc = Vector3.zero;
                checkForPlayer = StartCoroutine(CheckForPlayer());
            }
        }
    }

    private IEnumerator DelayOn()
    {
        while (rb.velocity.magnitude >= 0.1f)
        {
            yield return new WaitForSeconds(0.5f);

            if (rb.velocity.magnitude <= 0.1f)
            {
                delaying = false;
            }
        }
    }

    private void Shoot()
    {
        //grab dir and randomize power
        Vector2 shootDir = ((Vector2)playerLoc - (Vector2)transform.position).normalized;
        float shootPower = Random.Range(lowPower, highPower);

        //shoot
        rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);

        //Debug.Log(message: $"enemy shot at: {shootDir} with power: {shootPower}");ä
        //Debug.Log("Targeting");

        //reset timer and delay bool
        timer = 0f;
        delayLocationTrack = StartCoroutine(DelayOn());
    }
    
    private void Wander()
    {
        //grab dir and randomize power
        Vector2 shootDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        float shootPower = Random.Range(lowPower, highPower/2);

        //shoot
        rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);
        
        //Debug.Log(message: $"enemy shot at: {shootDir} with power: {shootPower}");
        //Debug.Log("Wandering");
        
        //reset timer and delay bool
        timer = 0f;
        delayLocationTrack = StartCoroutine(DelayOn());
    }
}
