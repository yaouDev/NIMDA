using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class Craft
    {
        public void CraftRecipe(Recipe recipe, Crafting crafting)
        {
            //Debug.Log("Available copper: " + crafting.copper);
            //Debug.Log("Available iron: " + crafting.iron);
            //Debug.Log("Available transistor(s): " + crafting.transistor);

            bool canCraft = true;

            if (recipe.product == null) Debug.LogWarning("Trying to craft null");

            //kan bytas ut mot en foreach loop ifall man vill ha many resources/anvaenda resource objekt
            if (crafting.copper < recipe.copperNeeded)
            {
                Debug.Log("Not enough copper!");
                canCraft = false;
            }
            if (crafting.iron < recipe.ironNeeded)
            {
                Debug.Log("Not enough iron!");
                canCraft = false;
            }
            if (crafting.transistor < recipe.transistorNeeded)
            {
                Debug.Log("Not enough transistors!");
                canCraft = false;
            }

            if (canCraft)
            {
                if (recipe.product.name.Equals("Battery"))
                {
                    crafting.copper -= recipe.copperNeeded;
                    crafting.iron -= recipe.ironNeeded;
                    crafting.transistor -= recipe.transistorNeeded;
                    crafting.CraftBattery();
                }

                if (recipe.product.name.Equals("Bullet"))
                {
                    crafting.iron--;
                    crafting.playerAttackScript.UpdateBulletCount(1);
                }
                else
                {
                    Debug.Log("You crafted: " + recipe.product + "!");
                    // TODO: Implement destination for crafted items
                }
                crafting.UpdateResources();
            }
        }
    }
}
