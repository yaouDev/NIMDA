using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft
{
    public void CraftRecipe(Recipe recipe, Crafting crafting)
    {
        Debug.Log("Available copper: " + crafting.copper);
        Debug.Log("Available iron: " + crafting.iron);
        Debug.Log("Available transistor(s): " + crafting.transistor);

        bool canCraft = true;

        if (recipe.product == null) Debug.LogWarning("Trying to craft null");

        //kan bytas ut mot en foreach loop ifall man vill ha många resources/använda resource objekt
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
            crafting.copper -= recipe.copperNeeded;
            crafting.iron -= recipe.ironNeeded;
            crafting.transistor -= recipe.transistorNeeded;

            if (recipe.product.name.Equals("Battery"))
            {
                //crafting.gameObject.GetComponent<BatteryUI>().AddBattery();
                Debug.Log("YOU CRAFTED A BATTERY, NOW GO CRAFT AN ASSAULT");
            }
            else
            {
                Debug.Log("You crafted: " + recipe.product + "!");
                Debug.Log("TODO: Implement desination for crafted items");
            }
        }
    }
}
