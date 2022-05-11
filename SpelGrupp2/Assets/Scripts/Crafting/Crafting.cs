using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CallbackSystem
{
    /*
     * Where the players inventory is instantiated, stored and managed.
     * Calls on method in PlayerAttack when bullet is crafted. 
     */
    public class Crafting : MonoBehaviour
    {
        [HideInInspector] public PlayerAttack playerAttackScript;
        [SerializeField] private Recipe batteryRecipe, bulletRecipe, UpgradedProjectileWeaponRecipe, UpgradedLaserWeaponRecipe;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject craftingTable;
        [SerializeField] private Button[] craftingButtons;
        private int[] resourceArray;
        private Button selectedButton;
        private float sphereRadius = 1f; 
        private float maxSphereDistance = 3f;
        private int selectedButtonIndex;

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
            resourceArray = new int[] { copper, transistor, iron };
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
            resourceArray = new int[] { copper, transistor, iron };

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

        //Priority on interactions
        //Interaction function should not be in Crafting script!
        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isCrafting)
                {
                    EnterCraftingUI();
                }
                else
                {
                    RaycastHit hit;
                    Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, maxSphereDistance, layerMask);
                    if (hit.collider != null)
                    {
                        Debug.Log("Collided with: " + hit.collider.gameObject.name);
                        EnterCraftingUI();
                    }
                }
            }

        }

        private void EnterCraftingUI()
        {
            isCrafting = !isCrafting;

            if (!isCrafting)
            {
                craftingTable.SetActive(false);
                selectedButton.image.color = Color.white;
                selectedButtonIndex = 0;
                playerInput.SwitchCurrentActionMap("Player");
            } 
            else
            {
                craftingTable.SetActive(true);
                selectedButton = craftingButtons[selectedButtonIndex];
                selectedButton.image.color = Color.red;
                playerInput.SwitchCurrentActionMap("Crafting");
            }
        }

        public void NextButton(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                selectedButtonIndex++;
                if (selectedButtonIndex == craftingButtons.Length) 
                        selectedButtonIndex = 0;

                ChangeSelectedButton();
            }
        }

        public void PreviousButton(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                selectedButtonIndex--;
                if (selectedButtonIndex < 0)
                    selectedButtonIndex = craftingButtons.Length-1;

                ChangeSelectedButton();
            }
        }

        private void ChangeSelectedButton()
        {
            selectedButton.image.color = Color.white;
            selectedButton = craftingButtons[selectedButtonIndex];
            selectedButton.image.color = Color.red;
        }

        public void SelectButton(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(selectedButton.interactable != false)
                {
                    selectedButton.onClick.Invoke();
                    selectedButton.interactable = false;
                }
                else
                    Debug.Log("Upgrade has already been applied");
            }
        }

        public void CraftBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(TryCraftRecipe(bulletRecipe))
                    playerAttackScript.UpdateBulletCount(1);

            }
        }
        public void CraftBattery(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(TryCraftRecipe(batteryRecipe))
                    playerHealthScript.IncreaseBattery();
            }
        }

        public void CraftUpgradedProjectileWeapon()
        {
            if (TryCraftRecipe(UpgradedLaserWeaponRecipe))
                playerAttackScript.UpgradeProjectileWeapon();
        }

        public void CraftUpgradedLaserWeapon()
        {
            if (TryCraftRecipe(UpgradedLaserWeaponRecipe))
                playerAttackScript.UpgradeLaserWeapon();
        }

        public bool TryCraftRecipe(Recipe recipe)
        {
            bool missingResources = false;
            if (recipe == null) Debug.LogWarning("Trying to craft null");

            for (int i = 0; i < recipe.ResNeededArr.Length; i++)
            {
                Debug.Log(recipe.ResNeededArr[i]);
                if (resourceArray[i] < recipe.ResNeededArr[i])
                {
                    Debug.Log("Not enough resources");
                    missingResources = true;
                }
            }

            if (!missingResources)
            {
                copper -= recipe.copperNeeded;
                iron -= recipe.ironNeeded;
                transistor -= recipe.transistorNeeded;
                UpdateResources();
                return true;
            }
            return false;
        }
    }   
}