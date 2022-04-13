using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //ta bort sen

public class Crafting : MonoBehaviour {
    public int copper = 10;
    public int transistor = 10;
    public int iron = 10;

    private Craft craft = new Craft();
    public Recipe batteryRecipe;


    void Start() {
        
    }
    
    void Update() {
        

    private bool[] validRecipe = {true, true};
    private enum scrap
    {
        copper,
        transistor,
        iron

    }

    public void CraftBattery(InputAction.CallbackContext context)
    {
        //byt ut hela j�vla metoden

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
}
