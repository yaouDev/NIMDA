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
        [SerializeField] private Recipe batteryRecipe, bulletRecipe, 
        UpgradedRevolverRecipe, UpgradedLaserWeaponRecipe, 
        ExplosionCritRevolverRecipe, cyanRecipe, yellowRecipe, whiteRecipe, 
        magentaRecipe, greenRecipe, blackRecipe;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject craftingTable;
        [SerializeField] private Button[] craftingButtons;
        [SerializeField] private Button defaultColorButton;
        [SerializeField][Range(1f, 20f)] private float newMovementSpeed = 7.5f;
        [SerializeField][Range(1f, 300f)] private float newHealth = 200f;

        private int[] resourceArray;
        private Button selectedButton;
        private float sphereRadius = 1f, maxSphereDistance = 3f;
        private int selectedButtonIndex;
        //Cyan, Yellow, Magenta, White, Black
        //private static bool[] colorsTakenArray = new bool[5];

        public int copper, transistor, iron, currency;

        private ResourceUpdateEvent resourceEvent;
        private FadingTextEvent fadingtextEvent;
        private PlayerHealth playerHealthScript;
        private PlayerController playerControllerScript;
        private bool isPlayerOne, started = false, isCrafting = false;
        private PlayerInput playerInput;

        public void UpdateResources()
        {
            resourceEvent.c = copper;
            resourceEvent.t = transistor;
            resourceEvent.i = iron;
            resourceEvent.currency = currency;
            resourceEvent.ammoChange = false;
            resourceArray = new int[] { copper, transistor, iron, currency };
            EventSystem.Current.FireEvent(resourceEvent);
        }


        public bool IsPlayerOne() { return isPlayerOne; }
        public int[] GetResourceArray() { return resourceArray; }

        private void Awake()
        {
            playerAttackScript = GetComponent<PlayerAttack>();
            playerHealthScript = GetComponent<PlayerHealth>();
            playerControllerScript = GetComponent<PlayerController>();
            playerInput = GetComponent<PlayerInput>();
            fadingtextEvent = new FadingTextEvent();
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
                fadingtextEvent.isPlayerOne = isPlayerOne;
                UpdateResources();
                started = true;
            }
        }

        //%-------------------------------Crafting table----------------------------------%

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isCrafting)
                    EnterCraftingUI();
                else
                {
                    RaycastHit hit;
                    Physics.SphereCast(transform.position, sphereRadius, transform.forward, 
                    out hit, maxSphereDistance, layerMask);
                    if (hit.collider != null)
                    {
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
                if (selectedButton.interactable)
                    selectedButton.onClick.Invoke();
                else
                {
                    fadingtextEvent.text = "Unavailable Purchase";
                    EventSystem.Current.FireEvent(fadingtextEvent);
                }
                    
            }
        }

        //%--------------------------------Crafts & upgrades---------------------------------%

        public void CraftBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (playerAttackScript.ReturnBullets() < playerAttackScript.ReturnMaxBullets())
                {
                    if (TryCraftRecipe(bulletRecipe))
                    {
                        playerAttackScript.UpdateBulletCount(3);
                        fadingtextEvent.text = "Bullets Crafted (x3)";
                        EventSystem.Current.FireEvent(fadingtextEvent);
                    }
                    else
                    {
                        Debug.Log("Carrying max bullets!");
                    }
                }
            }
        }
        public void CraftBattery(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (playerHealthScript.GetCurrentBatteryCount() < playerHealthScript.GetMaxBatteryCount())
                {
                    if (TryCraftRecipe(batteryRecipe))
                    {
                        playerHealthScript.IncreaseBattery();
                        fadingtextEvent.text = "Battery Crafted";
                        EventSystem.Current.FireEvent(fadingtextEvent);
                    }
                }
                else
                {
                    Debug.Log("Carrying max batteries!");
                }
            }
        }

        public void CraftCritableRevolver()
        {
            if (TryCraftRecipe(ExplosionCritRevolverRecipe))
            {
                //playerAttackScript.EnableExplosiveBullet();
                fadingtextEvent.text = "Revolver Crit Enabled";
                selectedButton.interactable = false;
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }
        public void CraftUpgradedRevolver()
        {
            if (TryCraftRecipe(UpgradedRevolverRecipe))
            {
                //playerAttackScript.UpgradeRevolver();
                fadingtextEvent.text = "Revolver Upgraded";
                selectedButton.interactable = false;
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftUpgradedLaserWeapon()
        {
            if (TryCraftRecipe(UpgradedLaserWeaponRecipe))
            {
                playerAttackScript.UpgradeLaserWeapon();
                fadingtextEvent.text = "Lasergun Upgraded";
                selectedButton.interactable = false;
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        //%-----------------------------------Colors------------------------------------%


        public void CraftMaterialColorGreen()
        {
            if (TryCraftRecipe(greenRecipe))
            {
                ResetPreviousStats();
                playerControllerScript.SetTerminalVelocity(newMovementSpeed);
                playerHealthScript.ChooseMaterialColor(new Color(0.35f, 0.95f, 0f, 1f));
                fadingtextEvent.text = "Color Green Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorYellow()
        {
            if (TryCraftRecipe(yellowRecipe))
            {
                ResetPreviousStats();
                playerControllerScript.SetTerminalVelocity(newMovementSpeed);
                playerHealthScript.ChooseMaterialColor(Color.yellow);
                fadingtextEvent.text = "Color Yellow Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorCyan()
        {
            if (TryCraftRecipe(cyanRecipe))
            {
                ResetPreviousStats();
                playerHealthScript.SetNewHealth(newHealth);
                playerHealthScript.ChooseMaterialColor(new Color(0.1f, 0.90f, 0.90f, 1f));
                fadingtextEvent.text = "Color Cyan Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorMagenta()
        {
            if (TryCraftRecipe(magentaRecipe))
            {
                ResetPreviousStats();
                playerHealthScript.SetNewHealth(newHealth);
                playerHealthScript.ChooseMaterialColor(new Color(0.85f, 0f, 0.85f, 1f));
                fadingtextEvent.text = "Color Magenta Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorWhite()
        {
            if (TryCraftRecipe(whiteRecipe))
            {
                ResetPreviousStats();
                playerHealthScript.ChooseMaterialColor(new Color(0.95f, 0.95f, 0.95f, 1f));
                fadingtextEvent.text = "Color White Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorBlack()
        {
            if (TryCraftRecipe(blackRecipe))
            {
                ResetPreviousStats();
                playerHealthScript.ChooseMaterialColor(new Color(0.25f, 0.25f, 0.25f, 1f));
                fadingtextEvent.text = "Color Black Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftDefaultColor()
        {
            ResetPreviousStats();
            playerHealthScript.ChooseMaterialColor();
            fadingtextEvent.text = "Default Color Crafted";
        }

        private void ResetPreviousStats()
        {
            playerHealthScript.SetDefaultStats();
        }

        //%-----------------------------------------------------------------------------%
        public bool TryCraftRecipe(Recipe recipe)
        {
            bool missingResources = false;
            if (recipe == null) Debug.LogWarning("Trying to craft null");

            for (int i = 0; i < recipe.ResNeededArr.Length; i++)
            {
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
                currency -= recipe.currencyNeeded;
                UpdateResources();
                if (selectedButton != defaultColorButton && isCrafting)
                    selectedButton.interactable = false;
                return true;
            }
            return false;
        }
        //%-------------------------------Forsaken methods-------------------------------%
        public int GetHalfResourceAmount(int index) { return resourceArray[index]; }
        public void ReduceResourceByHalf(int index)
        {
            if (index == 0)
                copper /= 2;
            else if (index == 1)
                transistor /= 2;
            else
                iron /= 2;
        }
    }   
}