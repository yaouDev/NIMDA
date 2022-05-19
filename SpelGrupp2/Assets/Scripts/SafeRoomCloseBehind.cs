using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SafeRoomCloseBehind : MonoBehaviour {

    [SerializeField] private float openHeight;
    [SerializeField] private float eventDuration = 10f;
    [SerializeField] private GameObject entrance;
    [SerializeField] private GameObject exit;
    private float timeElapsed;
    private bool doorOpen = true; 
    private int playerCount;
    private EnemySpawnController spawnController;
    private Compass compass;
    private Vector3 entranceClosePosition;
    private Vector3 entranceStartPosition;
    private Vector3 entranceOpenPosition;
    private Vector3 exitClosePosition;
    private Vector3 exitStartPosition;
    private Vector3 exitOpenPosition;
    private ObjectivesManager objectivesManager;
    private bool bossNext;

    // Start is called before the first frame update
    void Start() {
        entranceOpenPosition = entrance.transform.position;
        exitOpenPosition = exit.transform.position;
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
       // compass = GameObject.Find("Compass").GetComponent<Compass>();
        objectivesManager = GameObject.Find("ObjectivesManager").GetComponent<ObjectivesManager>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player")
        {
            playerCount++;
            //Debug.Log("Playercount = " + playerCount);
            if (playerCount == 2)
            {
                CloseEntrance();
                //compass.UpdateQuest();
                spawnController.GeneratorRunning(false);
                CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
                //Debug.Log("st�ng entrance" + playerCount);
                objectivesManager.RemoveObjective("enter safe room");
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerCount--;
            //Debug.Log("Playercount = " + playerCount);
            //Debug.Log("st�ng exit" + playerCount);
            if (playerCount < 1)
            {
                CloseExit();
                compass.UpdateQuest();
                if (!bossNext)
                {
                    objectivesManager.AddObjective("find the next safe room");
                    bossNext = true;
                }
                else
                {
                    objectivesManager.AddObjective("kill the boss");
                }
            }
        }
    }

    void CloseEntrance()
    {
        if (doorOpen)
        {
            //Debug.Log("Opening");
            entranceClosePosition = entranceOpenPosition + Vector3.down * openHeight;
            StartCoroutine(MoveEntrence(entranceClosePosition, eventDuration));
            spawnController.GeneratorRunning(false);
        }
    }
    void CloseExit()
    {
            //Debug.Log("Opening");
            exitClosePosition = exitOpenPosition + Vector3.down * openHeight;
            StartCoroutine(MoveExit(exitClosePosition, eventDuration));
    }

    IEnumerator MoveEntrence(Vector3 targetPosition, float duration)
    {
        timeElapsed = 0;
        entranceStartPosition = entrance.transform.position;
        while (entrance.transform.position != targetPosition)
        {
            //Debug.Log("Moving Door");
            entrance.transform.position = Vector3.Lerp(entranceStartPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorOpen = false;
    }
    IEnumerator MoveExit(Vector3 targetPosition, float duration)
    {
        timeElapsed = 0;
        exitStartPosition = exit.transform.position;
        while (exit.transform.position != targetPosition)
        {
            //Debug.Log("Moving Door");
            exit.transform.position = Vector3.Lerp(exitStartPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorOpen = false;
    }
}
