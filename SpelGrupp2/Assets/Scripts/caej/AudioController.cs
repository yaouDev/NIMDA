using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem; //ta bort??

public class AudioController : MonoBehaviour
{
    /* HOW TO USE THE AUDIO CONTROLLER WITH AUDIO CONTAINERS
     * 
     * [SerializeField] private PlayerAudioContainer player1; //allocate space for a container
     * AudioController.instance.PlayOneShotAttached(player1.fire1, gameObject); //call upon the audio controller, choose how to trigger the audio, and send the event through the container
     * 
     * ALTERNATIVELY USE THE REFERENCES STORED WITHIN THE AUDIO CONTROLLER SCRIPT
     * AudioController.instance.PlayerOneShotAttached(AudioController.instance.player1.fire1, gameObject);
    */

    /* TO CHANGE CHANNEL TIME ON LASER WEAPON
     * RuntimeManager.StudioSystem.setParameterByName("Channel Time", *INSERT VALUE*);
    */


    [Range(0, 24)]
    [SerializeField]
    public float night;

    public EventReference testReference;
    public Transform testPosition;

    public PlayerAudioContainer player1;
    public PlayerAudioContainer player2;
    public EnemyAudioContainer enemySound;

    public static AudioController instance;
    private void Awake()
    {
        instance ??= this;
    }

    private void Update()
    {
        //is there a better way to update this?
        RuntimeManager.StudioSystem.setParameterByName("Night", night);
    }

    public void PlayOneShot(EventReference audioEvent, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(audioEvent.Path, pos);
    }

    public void PlayOneShotAttatched(EventReference audioEvent, GameObject attached)
    {
        RuntimeManager.PlayOneShotAttached(audioEvent.Path, attached);
    }

    public FMOD.Studio.EventInstance PlayNewInstance(EventReference reference, GameObject attached)
    {
        FMOD.Studio.EventInstance ei = RuntimeManager.CreateInstance(reference);
        RuntimeManager.AttachInstanceToGameObject(ei, attached.transform);
        ei.start();
        return ei;
    }

    public FMOD.Studio.EventInstance PlayNewInstanceWithParameter(EventReference reference, GameObject attached, string pName, float pValue)
    {
        FMOD.Studio.EventInstance ei = RuntimeManager.CreateInstance(reference);
        RuntimeManager.AttachInstanceToGameObject(ei, attached.transform);
        ei.setParameterByName(pName, pValue);
        ei.start();
        return ei;
    }

    public void TriggerTest()
    {
        PlayOneShot(testReference, testPosition.position);
    }

    public EventReference FindEvent(string eventName, EventReference[] referenceArray)
    {
        foreach(EventReference er in referenceArray)
        {
            if (er.Path == "event:/" + eventName) return er;
        }

        Debug.LogWarning("No event with name: " + eventName);
        return new EventReference();
    }
}
