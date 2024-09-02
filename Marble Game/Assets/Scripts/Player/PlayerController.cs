using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Constant Variables")]
    private const int powerLimit = 5;

    [Header("Dynamic Variables")]
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private float pullLength;
    private Vector2 shootAngle;

    [Header("Engine Variables")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CircleCollider2D collider2d;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TouchTracking();
        Debug.Log(message: $"touchStart = {touchStart}, touchEnd = {touchEnd}");
    }

    private void TouchTracking()
    {

        foreach(Touch touch in Input.touches)
        {
            if (touch.fingerId != 0) break;

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = Input.touches[0].position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                touchEnd = Input.touches[0].position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                CalcStrength();
                ShootBall();
            }
        }
    }

    private void CalcStrength()
    {
        pullLength = Vector2.Distance(touchStart, touchEnd);
        if (pullLength > powerLimit) pullLength = 5f;

        shootAngle = (touchStart - touchEnd).normalized;
    }

    private void ShootBall()
    {
        rb.AddForce(shootAngle * pullLength);
    }
}
