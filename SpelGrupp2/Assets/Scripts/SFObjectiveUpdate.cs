using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFObjectiveUpdate : MonoBehaviour
{
    private ObjectivesManager objM;

    void Start()
    {
        objM = GameObject.Find("ObjectivesManager").GetComponent<ObjectivesManager>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            objM.RemoveObjective("find the first safe room");
            objM.RemoveObjective("find the next safe room");
            //objM.AddObjective("open the safe room");
            objM.AddObjective("start the generator");
            gameObject.SetActive(false);
        }
    }
}
