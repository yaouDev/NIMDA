using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Crafting : MonoBehaviour {
    public int copper = 1;
    public int transistor = 1;
    public int iron = 1;

    private Craft craft = new Craft();
    public Recipe batteryRecipe;
    private enum scrap
    {
        copper,
        transistor,
        iron
    }
    
    private scrap[][] combos = new scrap[][]
    {
        new scrap[] {scrap.copper, scrap.transistor, scrap.iron},
        new scrap[] {scrap.iron, scrap.iron, scrap.iron}
    };
    
    private int currentIndex = 0;
    private bool[] validRecipe = {true, true};

    public void PressedCopper(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Combo(scrap.copper);
        }
    }
    
    public void PressedIron(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Combo(scrap.iron);
        }
    }
    
    public void PressedTransistor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Combo(scrap.transistor);
        }
    }

    private void Combo(scrap latestPress)
    {
        for (int recipee = 0; recipee < validRecipe.Length; recipee++)
        {
            if (!validRecipe[recipee]) continue;
            if (recipee > combos[recipee].Length) continue;
            
            if (combos[recipee][currentIndex] == latestPress)
            {
                currentIndex++;
                if (currentIndex >= combos[recipee].Length)
                {
                    ResetValidRecipees();
                    SuccessfulCombo(recipee);
                    currentIndex = 0;
                    return;
                }
            }
            else
            {
                validRecipe[recipee] = false;
            }
        }
    }

    private void ResetValidRecipees()
    {
        currentIndex = 0;
        for (int recipe = 0; recipe < validRecipe.Length; recipe++)
        {
            validRecipe[recipe] = true;
        }
    }

    private void SuccessfulCombo(int recipee)
    {
        switch (recipee)
        {
            case (0):
                Debug.Log("crafted battery");
                //craft.CraftRecipe(batteryRecipe, this);
                break;
            case (1):
                Debug.Log("crafted bullet");
                //craft.CraftRecipe(batteryRecipe, this);
                break;
        }
    }
}
