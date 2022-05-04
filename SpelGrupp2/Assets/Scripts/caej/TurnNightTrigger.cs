using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnNightTrigger : MonoBehaviour
{
    public bool turnNight = true;
    public float transitionTime = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Ambience.instance.StartCoroutine(Ambience.instance.ChangeTime(turnNight, transitionTime));
            Ambience.instance.TriggerCrows();
        }
    }
}
