using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class Craft
    {
        public void CraftRecipe(Recipe recipe, Crafting crafting)
        {
            bool canCraft = true;
            if (recipe == null) Debug.LogWarning("Trying to craft null");
            int[] playerResources = crafting.GetResourceArray();

            for (int i = 0; i < recipe.ResNeededArr.Length; i++)
            {
                if (playerResources[i] < recipe.ResNeededArr[i])
                {
                    Debug.Log("Not enough resources");
                }
            }

            if(canCraft)
            {
                recipe.co.PerformAction(crafting);
                crafting.copper -= recipe.copperNeeded;
                crafting.iron -= recipe.ironNeeded;
                crafting.transistor -= recipe.transistorNeeded;
            }

        }
    }
}
