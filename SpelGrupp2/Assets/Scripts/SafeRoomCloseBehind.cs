using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoomCloseBehind : MonoBehaviour {
    private Vector3 entranceClosed;
    private Vector3 exitClosed;
    [SerializeField] private GameObject entrance;
    [SerializeField] private GameObject exit;
    private int playerCount;
    private EnemySpawnController spawnController;
    private Compass compass;

    // Start is called before the first frame update
    void Start() {
        entranceClosed = entrance.transform.position;
        exitClosed = exit.transform.position;
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
        compass = GameObject.Find("Compass").GetComponent<Compass>();
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
        entrance.transform.position = entranceClosed;
    }

    void CloseExit()
    {
        exit.transform.position = exitClosed;
    }
}
