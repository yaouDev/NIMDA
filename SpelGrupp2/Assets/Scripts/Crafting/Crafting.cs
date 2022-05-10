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
        [SerializeField] private Recipe batteryRecipe, bulletRecipe;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject craftingTable;
        private float sphereRadius = 1f; 
        private float maxSphereDistance = 3f;

        //public Recipe batteryRecipe, bulletRecipe;
        public int copper, transistor, iron;

        private ResourceUpdateEvent resourceEvent;
        private PlayerHealth playerHealthScript;
        private Craft craft = new Craft();
        private bool isPlayerOne, started = false, isCrafting = false;
        private PlayerInput playerInput;

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
            playerInput = GetComponent<PlayerInput>();
            resourceEvent = new ResourceUpdateEvent();
            craftingTable.SetActive(false);
        }

        public int[] GetResourceArray(){ return new int[] {copper, transistor, iron}; }


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

        //Priority on interactions
        //Interaction function should not be in Crafting script!
        public void Interact(InputAction.CallbackContext context)
        {
            RaycastHit hit;
            Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, maxSphereDistance, layerMask);
            if (hit.collider != null)
            {
                Debug.Log("Collided with: " + hit.collider.gameObject.name);
                EnterCraftingUI();
            }
        }

        private void EnterCraftingUI()
        {
            isCrafting = !isCrafting;

            if (!isCrafting)
            {
                craftingTable.SetActive(false);
                //playerInput.SwitchCurrentActionMap("Player");
            } 
            else
            {
                craftingTable.SetActive(true);
                //playerInput.SwitchCurrentActionMap("CraftingTable");
            }
        }


        public void CraftBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(craft.TryCraftRecipe(bulletRecipe, this))
                    playerAttackScript.UpdateBulletCount(1);

            }
        }
        public void CraftBattery(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(craft.TryCraftRecipe(batteryRecipe, this))
                    playerHealthScript.IncreaseBattery();
            }
        }


        /*

        public void CraftPlayerRecipe(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
               craft.CraftRecipe(recipe, this);
            }
        }


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