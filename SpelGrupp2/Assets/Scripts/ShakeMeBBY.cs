using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class ShakeMeBBY : MonoBehaviour
{
    private CallbackSystem.CameraShakeEvent shakeEvent = new CameraShakeEvent();
    [SerializeField] private bool shake;
    private bool playerOne;
    [SerializeField] [Range(0.0f, 1.0f)] private float magnitude = .5f;
    void Update()
    {
        if (shake)
        {
            shake = false;
            playerOne = !playerOne;
            
            shakeEvent.affectsPlayerOne = playerOne;
            shakeEvent.affectsPlayerTwo = !playerOne;
            shakeEvent.magnitude = magnitude;
            EventSystem.Current.FireEvent(shakeEvent);
        }
    }
}
