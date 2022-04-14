using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem; //ta bort??

public class AudioController : MonoBehaviour
{
    private AudioController instance;

    public FMOD.Studio.EventInstance eventInstance;


    private void Awake()
    {
        instance ??= this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayOneShot(string path, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(path, pos);
    }

    public void TriggerTest(InputAction.CallbackContext ctx)
    {
        //if (ctx.performed) 
    }
}
