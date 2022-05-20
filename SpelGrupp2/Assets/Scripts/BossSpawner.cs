using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class BossSpawner : MonoBehaviour {
    [SerializeField] private GameObject boss;
    private int playerCount;
    EnemySpawnController spawner;

    // Start is called before the first frame update
    void Start() {
        spawner = GameObject.FindObjectOfType<EnemySpawnController>();
        // boss = GameObject.Find("Boss");
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            playerCount++;
            if (playerCount == 2) {
                boss.SetActive(true);
                gameObject.SetActive(false);
                EventSystem.Current.FireEvent(new SafeRoomEvent());
                spawner.gameObject.SetActive(false);
            }
        }
    }
}
