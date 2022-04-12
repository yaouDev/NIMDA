using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{
    public void CraftRecipe(Recipe recipe, Crafting crafting)
    {
        if (recipe.product == null) Debug.LogWarning("Trying to craft null");

        if (crafting.copper < recipe.copperNeeded) Debug.Log("Not enough copper!");
        else if (crafting.iron < recipe.ironNeeded) Debug.Log("Not enough iron!");
        else if (crafting.transistor < recipe.transistorNeeded) Debug.Log("Not enough transistors!");
        else
        {
            crafting.copper -= recipe.copperNeeded;
            crafting.iron -= recipe.ironNeeded;
            crafting.transistor -= recipe.transistorNeeded;

            //craft
        }


    }

}
