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
        [HideInInspector] public PlayerAttack playerAttackScript;
        //public Recipe batteryRecipe, bulletRecipe;
        public int copper, transistor, iron;

        private ResourceUpdateEvent resourceEvent;
        private PlayerHealth playerHealthScript;
        private Craft craft = new Craft();
        private bool isPlayerOne, started = false;

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

        public int[] GetResourceArray(){ return new int[] {copper, transistor, iron}; }

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

        public void PressedBattery(InputAction.CallbackContext context, Recipe recipe)
        {
            if (context.performed)
            {
                craft.CraftRecipe(recipe, this);
            }
        }

        /*
        public void PressedBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                craft.CraftRecipe(bulletRecipe, this);
            }
        }
        */
    }
}