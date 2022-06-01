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
    private Vector3 entranceClosePosition;
    private Vector3 entranceStartPosition;
    private Vector3 entranceOpenPosition;
    private Vector3 exitClosePosition;
    private Vector3 exitStartPosition;
    private Vector3 exitOpenPosition;
    private ObjectivesManager objectivesManager;


    // Start is called before the first frame update
    void Start() {
        entranceOpenPosition = entrance.transform.position;
        exitOpenPosition = exit.transform.position;
        spawnController = FindObjectOfType<EnemySpawnController>();
        // compass = GameObject.Find("Compass").GetComponent<Compass>();
        objectivesManager = FindObjectOfType<ObjectivesManager>();
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            playerCount++;
            if (playerCount == 2) {
                CloseEntrance();
                //compass.UpdateQuest();
                spawnController.GeneratorRunning(false);
                CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
                //Debug.Log("st�ng entrance" + playerCount);
                SaveSystem.Instance.SaveGameData(true);
            }
        }
    }
    private void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Player") {
            playerCount--;
            //Debug.Log("Playercount = " + playerCount);
            //Debug.Log("st�ng exit" + playerCount);
            if (playerCount == 0) {
                CloseExit();
                // compass.UpdateQuest();
                if (!objectivesManager.BossNext()) {
                    objectivesManager.AddObjective("find the next safe room");
                    objectivesManager.FlipBossBool();
                    spawnController.gameObject.SetActive(true);
                    spawnController.StartCoroutine(spawnController.SpawnObject());
                } else {
                    objectivesManager.AddObjective("kill the boss");

                }
                SaveSystem.Instance.SaveGameData(false);
            }
        }
    }

    void CloseEntrance() {
        if (doorOpen) {
            entranceClosePosition = entranceOpenPosition + Vector3.down * openHeight;
            StartCoroutine(MoveEntrance(entranceClosePosition, eventDuration));
            spawnController.GeneratorRunning(false);
            spawnController.gameObject.SetActive(false);
            objectivesManager.RemoveObjective("enter safe room");
        }
    }
    void CloseExit() {
        //Debug.Log("Opening");
        exitClosePosition = exitOpenPosition + Vector3.down * openHeight;
        StartCoroutine(MoveExit(exitClosePosition, eventDuration));
        spawnController.gameObject.SetActive(true);
    }

    IEnumerator MoveEntrance(Vector3 targetPosition, float duration) {
        timeElapsed = 0;
        entranceStartPosition = entrance.transform.position;
        while (timeElapsed < 1.0f) {
            entrance.transform.position = Vector3.Lerp(entranceStartPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * (1.0f / duration);
            yield return null;
        }

        entrance.transform.position = targetPosition;
        doorOpen = false;
    }
    IEnumerator MoveExit(Vector3 targetPosition, float duration) {
        timeElapsed = 0;
        exitStartPosition = exit.transform.position;
        while (timeElapsed < 1.0f) {
            exit.transform.position = Vector3.Lerp(exitStartPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * (1.0f / duration);
            yield return null;
        }

        exit.transform.position = targetPosition;
        doorOpen = false;
    }
}
