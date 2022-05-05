using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

namespace CallbackSystem
{
    public class Crafting : MonoBehaviour
    {
        public int copper = 1;
        public int transistor = 1;
        public int iron = 1;

        private Craft craft = new Craft();
        public Recipe batteryRecipe;
        public Recipe bulletRecipe;
        private ResourceUpdateEvent resEvent;
        private bool isPlayerOne;
        private bool started = false;

        public enum PickUp
        {
            Copper,
            Transistor,
            Iron
        }

        private void Start()
        {
            isPlayerOne = GetComponent<PlayerHealth>().IsPlayerOne();
        }
        //TODO Rewrite shit-solution so the inventory is updated at start
        private void Update()
        {
            if (!started)
            {
                resEvent = new ResourceUpdateEvent();
                resEvent.c = copper;
                resEvent.t = transistor;
                resEvent.i = iron;
                resEvent.isPlayerOne = isPlayerOne;
                EventSystem.Current.FireEvent(resEvent);
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

        private void SuccessfulCombo(int recipee)
        {
            switch (recipee)
            {
                case (0):
                    //Debug.Log("crafted battery");
                    //batteryUI.AddBattery();
                    craft.CraftRecipe(batteryRecipe, this);
                    break;
                case (1):
                    //Debug.Log("crafted bullet");
                    craft.CraftRecipe(bulletRecipe, this);
                    resEvent.isPlayerOne = isPlayerOne;
                    resEvent.a++;
                    EventSystem.Current.FireEvent(resEvent);
                    break;
            }
        }
    }
}