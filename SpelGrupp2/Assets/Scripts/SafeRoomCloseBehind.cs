using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using CallbackSystem;

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

    [SerializeField] private GameObject doorEntranceSource;
    [SerializeField] private GameObject doorExitSource;
    [SerializeField] private EventReference doorSound;
    private FMOD.Studio.EventInstance doorEvent;
    private AudioController ac;
    
    private string player = "Player";
    private GameObject[] players;
    private Dictionary<GameObject, bool> entered = new Dictionary<GameObject, bool>();
    private bool visited = false;
    private bool exited = false;

    void Start() {
        PlayerHealth[] playerHealths = FindObjectsOfType<PlayerHealth>();
        players = new GameObject[playerHealths.Length];
        
        for (int i = 0; i < playerHealths.Length; i++)
        {
            players[i] = playerHealths[i].transform.gameObject;
            entered.Add(players[i], false);
        }
        
        entranceOpenPosition = entrance.transform.position;
        exitOpenPosition = exit.transform.position;
        spawnController = FindObjectOfType<EnemySpawnController>();
        // compass = GameObject.Find("Compass").GetComponent<Compass>();
        objectivesManager = FindObjectOfType<ObjectivesManager>();

        ac = AudioController.instance;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag.Equals(player))
        {
            entered[col.gameObject] = true; // adds colliding player to dictionary
            if (entered[players[0]] && entered[players[1]])
            {
                visited = true;
                CloseEntrance();
                spawnController.GeneratorRunning(false);
                CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
                SaveSystem.Instance.SaveGameData(true);
            }
        }
        
        //if (col.gameObject.tag == player) {
        //    playerCount++;
        //    if (playerCount == 2) {
        //        CloseEntrance();
        //        spawnController.GeneratorRunning(false);
        //        CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
        //        SaveSystem.Instance.SaveGameData(true);
        //    }
        //}
    }
    
    private void OnTriggerExit(Collider col) {
        if (col.gameObject.tag.Equals(player))
        {
            entered[col.gameObject] = false; // removes colliding player from dictionary
            if (visited && !exited && !entered[players[0]] && !entered[players[1]])
            {
                exited = true;
                CloseExit();
                if (!objectivesManager.BossNext()) 
                {
                    objectivesManager.AddObjective("find the next safe room");
                    objectivesManager.FlipBossBool();
                    spawnController.gameObject.SetActive(true);
                    spawnController.StartCoroutine(spawnController.SpawnObject());
                }
                else 
                {
                    objectivesManager.AddObjective("kill the boss");
                }
                SaveSystem.Instance.SaveGameData(false);
            }
        }
        
        // if (col.gameObject.tag == player) {
        //     playerCount--;
        //     if (playerCount == 0) {
        //         CloseExit();
        //         if (!objectivesManager.BossNext()) {
        //             objectivesManager.AddObjective("find the next safe room");
        //             objectivesManager.FlipBossBool();
        //             spawnController.gameObject.SetActive(true);
        //             spawnController.StartCoroutine(spawnController.SpawnObject());
        //         } else {
        //             objectivesManager.AddObjective("kill the boss");
        // 
        //         }
        //         SaveSystem.Instance.SaveGameData(false);
        //     }
        // }
    }

    void CloseEntrance() {
        if (doorOpen) {
            entranceClosePosition = entranceOpenPosition + Vector3.down * openHeight;
            StartCoroutine(MoveEntrance(entranceClosePosition, eventDuration));
            doorEvent = ac.PlayNewInstanceWithParameter(doorSound, doorSoundSource, "isOpen", 0f); //play door sound
            spawnController.GeneratorRunning(false);
            spawnController.gameObject.SetActive(false);
            objectivesManager.RemoveObjective("enter safe room");
        }
    }
    
    void CloseExit() {
        //Debug.Log("Opening");
        exitClosePosition = exitOpenPosition + Vector3.down * openHeight;
        doorEvent = ac.PlayNewInstanceWithParameter(doorSound, doorSoundSource, "isOpen", 0f); //play door sound
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
        doorEvent.setParameterByName("isOpen", 1f); //stop door sound
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
        doorEvent.setParameterByName("isOpen", 1f); //stop door sound
        doorOpen = false;
    }
}
