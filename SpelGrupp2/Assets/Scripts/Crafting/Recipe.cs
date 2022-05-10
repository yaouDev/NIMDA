using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    public int copperNeeded, transistorNeeded, ironNeeded;
    [HideInInspector] public int[] ResNeededArr;

    private void Awake(){ ResNeededArr = new int[]{copperNeeded, transistorNeeded, ironNeeded}; }
}