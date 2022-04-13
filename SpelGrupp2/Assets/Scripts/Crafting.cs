using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem; //ta bort sen

public class Crafting : MonoBehaviour {
    public int copper = 1;
    public int transistor = 1;
    public int iron = 1;

    private Craft craft = new Craft();
    public Recipe batteryRecipe;
        

    private bool[] validRecipe = {true, true};

 
    private enum scrap
    {
        copper,
        transistor,
        iron




    }

    private scrap[,] combos =
    {

      

        {scrap.copper, scrap.transistor, scrap.iron},
        {scrap.iron, scrap.iron, scrap.iron}
    };
    public int currentIndex = 0;


    public void CraftBattery(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            craft.CraftRecipe(batteryRecipe, this);
        }
    }



    private void Combo(scrap latestCombo)
    {
        if (true)
            return;
    }

    private void Combo()
    {
        
    }
}
