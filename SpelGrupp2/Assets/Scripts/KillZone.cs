using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            Debug.Log("entered");
            c.GetComponent<CallbackSystem.PlayerHealth>().Die();
        }
    }
}
