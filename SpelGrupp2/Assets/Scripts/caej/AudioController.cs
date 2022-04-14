using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem; //ta bort??

public class AudioController : MonoBehaviour
{
    public EventReference testReference;
    public Transform testPosition;

    public static AudioController instance;
    private void Awake()
    {
        instance ??= this;
    }

    public void PlayOneShot(string path, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(path, pos);
    }

    public void TriggerTest()
    {
        PlayOneShot(testReference.Path, testPosition.position);
    }
}
