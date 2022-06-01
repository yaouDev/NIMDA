using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "New Player Audio Container", menuName = "Audio/Audio Container/Player Container")]
public class PlayerAudioContainer : AudioContainer
{
    [Header("Movement")]
    public EventReference footstep;
    public EventReference foley;
    public EventReference idle;

    [Header("Combat")]
    public EventReference fire1;
    public EventReference fire2;
    public EventReference hurt;
    public EventReference dodge;
    public EventReference death;
    public EventReference batteryDelpetion;
    public EventReference laserNoAmmo;
    public EventReference projectileNoAmmo;

}
