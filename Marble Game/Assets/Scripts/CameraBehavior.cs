using Cinemachine;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [Header("Engine Variables")]
    private GameObject goal;
    private GameObject player;
    private CinemachineVirtualCamera cam;
    private CinemachineFramingTransposer transposer;

    [Header("Dynamic Variables")]
    private Vector3 playerStartPos;
    private Vector3 goalPos;

    private void Start()
    {
        goal = FindObjectOfType(typeof(goalManager)) as GameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        cam = gameObject.GetComponent<CinemachineVirtualCamera>();
        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();

        SavePositions();
    }

    private void Update()
    {
        if (goal == null)
        {
            goal = FindObjectOfType(typeof(goalManager)) as GameObject;
        }

        //CAM SCREEN Y VALUE NEEDS TO BE BETWEEN 0.5 AND 0.75

        //transposer.m_ScreenY =
        Debug.Log(message: $"0.5 - 0.75 scaled value between goal and player start is {CalcOffset()}");
    }

    private void SavePositions()
    {
        playerStartPos = player.transform.position;
        goalPos = goal.transform.position;
    }

    private float CalcOffset()
    {
        //PROJEKTOI VEKTORI PELAAJAN JA MAALIN VÄLILLÄ VEKTORIIN PELAAJAN ALKUPISTEEN JA MAALIN VÄLILLÄ
        //LASKE TÄMÄN JÄLKEEN KUINKA PITKÄN MATKAN PELAAJA ON KULKENUT TOTAL MATKASTA
        //SKAALAA TÄMÄ ARVO OLEMAAN VÄLILLÄ 0.5F JA 0.75F

        return 1;
    }
}
