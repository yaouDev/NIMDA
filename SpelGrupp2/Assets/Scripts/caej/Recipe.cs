using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    public int copperNeeded;
    public int ironNeeded;
    public int transistorNeeded;

    public Resource product;

}
