using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

namespace CallbackSystem
{
    /*
     * Where the players inventory is instantiated, stored and managed.
     * Calls on method in PlayerAttack when bullet is crafted. 
     */
    public class Crafting : MonoBehaviour
    {
        public int copper;
        public int transistor;
        public int iron;

        private Craft craft = new Craft();
        public Recipe batteryRecipe;
        public Recipe bulletRecipe;
        private ResourceUpdateEvent resourceEvent;
        public PlayerAttack playerAttackScript;
        private PlayerHealth playerHealthScript;
        private bool isPlayerOne;
        private bool started = false;

        public enum PickUp
        {
            Copper,
            Transistor,
            Iron
        }

        public void UpdateResources()
        {
            resourceEvent.c = copper;
            resourceEvent.t = transistor;
            resourceEvent.i = iron;
            resourceEvent.ammoChange = false;
            EventSystem.Current.FireEvent(resourceEvent);
        }


        public bool IsPlayerOne() { return isPlayerOne; }

        private void Awake()
        {
            playerAttackScript = GetComponent<PlayerAttack>();
            playerHealthScript = GetComponent<PlayerHealth>();
            resourceEvent = new ResourceUpdateEvent();
        }

        public void CraftBattery()
        {
            playerHealthScript.IncreaseBattery();
        }
        private void Update()
        {
            if (!started)
            {
                isPlayerOne = playerAttackScript.IsPlayerOne();
                resourceEvent.isPlayerOne = isPlayerOne;
                UpdateResources();
                started = true;
            }
        }
 
        private static readonly PickUp[][] Combos =
    {
        new PickUp[] {PickUp.Copper, PickUp.Transistor, PickUp.Iron},
        new PickUp[] {PickUp.Iron, PickUp.Iron, PickUp.Iron}
    };

        private int currentIndex = 0;
        private bool[] validRecipe = { true, true };

        public void PressedCopper(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Combo(PickUp.Copper);
            }
        }

        public void PressedIron(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Combo(PickUp.Iron);
            }
        }

        public void PressedTransistor(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Combo(PickUp.Transistor);
            }
        }

        public void PressedBattery(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                craft.CraftRecipe(batteryRecipe, this);
            }
        }

        public void PressedBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                craft.CraftRecipe(bulletRecipe, this);

            }
        }

        private void Combo(PickUp latestPress)
        {
            bool correctSoFar = false;
            for (int recipee = 0; recipee < validRecipe.Length; recipee++)
            {
                if (!validRecipe[recipee]) continue;
                if (recipee > Combos[recipee].Length) continue;

                if (Combos[recipee][currentIndex] == latestPress)
                {
                    correctSoFar = true;
                    currentIndex++;
                    if (currentIndex >= Combos[recipee].Length)
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

            if (!correctSoFar)
            {
                Debug.LogWarning("INCORRECT! START AGAIN!");
                ResetValidRecipees();
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

        public void CraftAmmunition()
        {
            
        }

        private void SuccessfulCombo(int recipee)
        {
            switch (recipee)
            {
                case (0):
                    craft.CraftRecipe(batteryRecipe, this);
                   // playerAttackScript.UpdateBulletCount(1);
                    break;
                case (1):
                    //Debug.Log("crafted bullet");
                    craft.CraftRecipe(bulletRecipe, this);
                    break;
            }
        }
    }
}