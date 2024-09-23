using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Engine Variables")]
    //[SerializeField] private GameManager gameManager;
    private Rigidbody2D rb;
    private GameObject player;

    [Header("Dynamic Variables")]
    private Vector3 playerLoc = Vector3.zero;
    private bool seesPlayer = false;
    private bool timing = false;
    private bool delaying = false;
    private float timer = 0f;

    private float lowPower = 5f;
    private float highPower = 25f;

    //randomize this based on current level -> smaller value = bigger difficulty
    [SerializeField][Range(1.0f, 3.0f)] private float delayBeforeShoot = 3f;

    //if player closer than this to player, enemy starts chasing, adjustable
    [SerializeField][Range(5.0f, 15.0f)] private float detectDistance = 10f;

    //Coroutines
    private Coroutine checkForPlayer;
    private Coroutine delayLocationTrack;

    private void Awake()
    {
        //fetch variables
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        checkForPlayer = StartCoroutine(CheckForPlayer());
    }

    void FixedUpdate()
    {
        //timer
        if (timing)
        {
            timer += Time.deltaTime;
        }

        //if timer high enough, shoot
        if (timer > delayBeforeShoot)
        {
            Shoot();
        }

        //if stopped and bool on, grab location
        if (!delaying)
        {
            playerLoc = player.transform.position;
            delaying = true;
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
        Vector2 shootDir = (playerLoc - gameObject.transform.position).normalized;
        float shootPower = Random.Range(lowPower, highPower);

        //shoot
        rb.AddForce(shootDir * shootPower, ForceMode2D.Impulse);

        //reset timer and delay bool
        timer = 0f;
        delayLocationTrack = StartCoroutine(DelayOn());
    }
}
