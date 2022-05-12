using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoomCloseBehind : MonoBehaviour {
    private Vector3 entranceClosed;
    private Vector3 exitClosed;
    [SerializeField] private GameObject entrance;
    [SerializeField] private GameObject exit;
    private EnemySpawnController spawnController;

    // Start is called before the first frame update
    void Start() {
        entranceClosed = entrance.transform.position;
        exitClosed = exit.transform.position;
        spawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
    }

    void OnTriggerEnter(Collider col) {
        entrance.transform.position = entranceClosed;
        exit.transform.position = exitClosed;
        spawnController.GeneratorRunning(false);
        CallbackSystem.EventSystem.Current.FireEvent(new CallbackSystem.SafeRoomEvent());
    }

}
