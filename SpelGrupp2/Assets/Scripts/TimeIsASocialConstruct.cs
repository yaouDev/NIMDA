using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIsASocialConstruct : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    private float timeODay = 0.0f;
    [SerializeField] private float timeSpeed = .1f;

    private void Start()
    {
        audioController = FindObjectOfType<AudioController>();
    }

    private void Update()
    {
        timeODay += Time.deltaTime * timeSpeed;
        // Christoffers grej?
        if (timeODay > 24) timeODay -= 24;
        audioController.night = timeODay;
    }
}
