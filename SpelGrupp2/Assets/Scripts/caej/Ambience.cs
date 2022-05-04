using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Ambience : MonoBehaviour
{
    public static Ambience instance;

    [SerializeField] private EventReference crows;
    /*
    [SerializeField] private EventReference wind;
    [SerializeField] private EventReference crickets;

    private FMOD.Studio.EventInstance windEvent;
    private FMOD.Studio.EventInstance cricketsEvent;*/

    private void Awake()
    {
        instance ??= this;
    }

    [Range(0, 24)]
    [SerializeField] private float night;

    // Start is called before the first frame update
    void Start()
    {
        /*
        cricketsEvent = RuntimeManager.CreateInstance(crickets);

        windEvent = RuntimeManager.CreateInstance(wind);
        windEvent.start();*/
    }

    void Update()
    {
        //is there a better way to update this?
        RuntimeManager.StudioSystem.setParameterByName("Night", night);
    }

    public void TriggerCrows()
    {
        FMOD.Studio.EventInstance crowsEvent = RuntimeManager.CreateInstance(crows);
        crowsEvent.start();
    }

    /*
    public IEnumerator ChangeTime(bool turnNight, float transitionTime)
    {
        print("Changing Time!");

        float end = Time.realtimeSinceStartup + transitionTime;

        while(Time.realtimeSinceStartup < end)
        {
            if (turnNight)
            {
                night += Time.deltaTime * 24 / transitionTime;

                //Activate night events
                PlayIfNotPlaying(cricketsEvent);
            }
            else
            {
                night -= Time.deltaTime / transitionTime;

                //Activate day events
            }
            yield return null;
        }

        if (turnNight)
        {
            //Deactivate day events

        }
        else
        {
            //Deactivate night events
            cricketsEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        }
    }

    private void PlayIfNotPlaying(FMOD.Studio.EventInstance m_event)
    {
        m_event.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE eventState);
        if (eventState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            m_event.start();
        }
        else
        {
            Debug.LogWarning(m_event.ToString() + " is playing!!");
        }
    }*/
}
