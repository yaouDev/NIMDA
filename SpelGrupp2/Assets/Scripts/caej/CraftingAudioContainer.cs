using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "New Crafting Container", menuName = "Audio/Audio Container/Crafting Container")]
public class CraftingAudioContainer : AudioContainer
{
    public EventReference batteryCraft;
    public EventReference ammoCraft;
    public EventReference craftTable;
}
