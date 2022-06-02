using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "New Audio Container", menuName = "Audio/Audio Container/Enemy Audio Container")]
public class EnemyAudioContainer : AudioContainer
{
    [Header("Combat")]
    public EventReference fire1;
    public EventReference melee;
    public EventReference hurt;
    public EventReference death;
    public EventReference explosion;

    [Header("Foley")]
    public EventReference footstep;
}
