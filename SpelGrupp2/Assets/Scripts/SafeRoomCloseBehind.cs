using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoomCloseBehind : MonoBehaviour {
    private Vector3 entranceClosed;
    private Vector3 exitClosed;
    [SerializeField] private GameObject entrance;
    [SerializeField] private GameObject exit;
    [SerializeField] private float entranceOpenTime;
    [SerializeField] private float entranceCloseTime = 10f;
    [SerializeField] private float exitOpenTime;
    [SerializeField] private float exitCloseTime = 10f;
    private float elapsedTime = 0;
    private float interpolationRatio; 

    private int playerCount;
    private EnemySpawnController spawnController;
    private Compass compass;
    private Vector3 interpolatedPosition;

    // Start is called before the first frame update
    void Start() {
        entranceClosed = entrance.transform.position;
        exitClosed = exit.transform.position;
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
        compass = GameObject.Find("Compass").GetComponent<Compass>();
    }
    private void FixedUpdate()
    {
        interpolationRatio = elapsedTime / entranceOpenTime * Time.deltaTime;
        interpolationRatio = elapsedTime / entranceCloseTime * Time.deltaTime;
        interpolationRatio = elapsedTime / exitOpenTime * Time.deltaTime;
        interpolationRatio = elapsedTime / exitCloseTime * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player")
        {
            playerCount++;
            //Debug.Log("Playercount = " + playerCount);
            if (playerCount == 2)
            {
                CloseEntrance();
                compass.UpdateQuest();
                spawnController.GeneratorRunning(false);
                CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCount--;
            //Debug.Log("Playercount = " + playerCount);
            if(playerCount == 0)
            {
                CloseExit();
            }
        }
    }

    void CloseEntrance()
    {
        interpolatedPosition = Vector3.Lerp(entrance.transform.position, entranceClosed, interpolationRatio);
        //entrance.transform.position = entranceClosed;
    }

    void CloseExit()
    {
        interpolatedPosition = Vector3.Lerp(exit.transform.position, exitClosed, interpolationRatio);
        //exit.transform.position = exitClosed;
    }
}
