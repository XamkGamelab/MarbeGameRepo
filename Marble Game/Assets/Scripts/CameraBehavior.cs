using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [Header("Engine Variables")]
    private goalManager goal;
    private GameObject player;
    private CinemachineVirtualCamera cam;
    private CinemachineFramingTransposer transposer;

    [Header("Dynamic Variables")]
    private Vector3 playerStartPos;
    private Vector3 goalPos;
    Vector2 goalVector;
    private float distToGoal;
    private float offset;

    [Header("Const Variables")]
    private const float minScaledOffset = 0.5f;
    private const float maxScaledOffset = 0.75f;

    private void Start()
    {
        //grab references
        goal = GameObject.FindObjectOfType<goalManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        cam = gameObject.GetComponent<CinemachineVirtualCamera>();
        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();

        //uncommenting will cause a nullref exception, use only if testing
        // SavePositions();
    }

    private void Update()
    {
        offset = CalcOffset();

        //avoid nullref memes
        if (offset > 0.5f && offset < 0.75f)
        {
            Debug.Log(message: $"0.5 - 0.75 scaled value between goal and player start is {offset}");
            transposer.m_ScreenY = offset;
        }
    }

    //exactly as stated, saved various positions used for offset calculations
    //also runs some calcs which do not need to be in update
    public void SavePositions()
    {
        goal = GameObject.FindObjectOfType<goalManager>();
        playerStartPos = player.transform.position;
        goalPos = goal.transform.position;
        goalVector = goalPos - playerStartPos;
        distToGoal = goalVector.magnitude;
    }

    private float CalcOffset()
    {
        //how far player has traveled so far, as a vector
        Vector2 posVector = player.transform.position - playerStartPos;

        //old debug log
        // Debug.Log(message: $"goalVector {goalVector}, posVector {posVector}");

        //project player's current position vector to goalVector (distance from start to goal)
        float progress = Helpers.Project(goalVector, posVector);

        //set value to be between 0-1 to ease next calc
        float scaledValue = progress / distToGoal;

        //scale value to be between 0.5 and 0.75
        scaledValue = minScaledOffset + (maxScaledOffset - minScaledOffset) * scaledValue;

        //reverse the scaling: 0.75 at start and 0.5 at end
        scaledValue = minScaledOffset + maxScaledOffset - scaledValue;

        //return clamped value so it always stays between min and max
        return Mathf.Clamp(scaledValue, minScaledOffset, maxScaledOffset);
    }
}
