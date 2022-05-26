using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPickup : MonoBehaviour
{
    private Follow parent;
    private CallbackSystem.PlayerHealth[] players;
    private GameObject closestPlayer;

    private void Start()
    {
        parent = GetComponentInParent<Follow>();
        players = FindObjectsOfType<CallbackSystem.PlayerHealth>();

    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            closestPlayer = 
                Vector3.Distance(parent.gameObject.transform.position, players[0].gameObject.transform.position) < 
                Vector3.Distance(parent.gameObject.transform.position, players[1].gameObject.transform.position) ? 
                players[0].gameObject : players[1].gameObject;

            parent.StartFollowing(closestPlayer);
        }
    }
}
