using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

public class TriggerBossCameraState : MonoBehaviour
{
    private string player = "Player";
    private CallbackSystem.BossRoomEvent bossRoomEvent = new BossRoomEvent();
    private GameObject[] players;
    private Dictionary<GameObject, bool> entered = new Dictionary<GameObject, bool>();

    private void Awake()
    {
        PlayerHealth[] playerHealths = FindObjectsOfType<PlayerHealth>();
        players = new GameObject[playerHealths.Length];
        
        for (int i = 0; i < playerHealths.Length; i++)
        {
            players[i] = playerHealths[i].transform.gameObject;
            entered.Add(players[i], false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("in trigger" + other.gameObject.name);
        if (other.gameObject.tag == player)
        {
            Debug.Log($"{other.gameObject.name} entered bossroom");
            entered[other.gameObject] = true;
            if (entered[players[0]] && entered[players[1]])
            {
                Debug.Log("BOTH ENTERED");
                bossRoomEvent.insideBossRoom = true;
                EventSystem.Current.FireEvent(bossRoomEvent);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == player)
        {
            Debug.Log($"{other.gameObject.name} exited bossroom");
            entered[other.gameObject] = false;
            if (!entered[players[0]] && !entered[players[1]])
            {
                bossRoomEvent.insideBossRoom = false;
                EventSystem.Current.FireEvent(bossRoomEvent);
            }
        }
    }
}
