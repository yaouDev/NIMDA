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
        [HideInInspector] public Blink playerBlinkScript;
        [SerializeField]
        private Recipe batteryRecipe, bulletRecipe,
        UpgradedProjectileWeaponRecipe, UpgradedLaserWeaponRecipe,
        cyanRecipe, yellowRecipe, whiteRecipe, magentaRecipe,
        greenRecipe, blackRecipe, RevolverCritRecipe, laserbeamWidthRecipe,
        largeMagazineRecipe, laserbeamChargeRecipe, blinkUpgradeRecipe;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject craftingTable;
        private int[] resourceArray;
        private float sphereRadius = .45f;
        private float maxSphereDistance = 3f;
        [SerializeField] private GameObject[] dropTable = new GameObject[4];

        public int copper, transistor, iron, currency;

        private ResourceUpdateEvent resourceEvent;
        private FadingTextEvent fadingtextEvent;
        private CraftingEvent craftingEvent;
        private PlayerHealth playerHealthScript;
        private PlayerController playerControllerScript;
        private bool isPlayerOne, started = false, isCrafting = false;
        private PlayerInput playerInput;

        public Dictionary<string, bool> colorDictionary;

        private Color blackColor = new Color(0.25f, 0.25f, 0.25f, 1f),
            greenColor = new Color(0.35f, 0.95f, 0f, 1f),
            cyanColor = new Color(0.1f, 0.90f, 0.90f, 1f),
            whiteColor = new Color(0.95f, 0.95f, 0.95f, 1f),
            magentaColor = new Color(0.85f, 0f, 0.85f, 1f),
            defaultColor;




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

        private void Awake()
        {
            if (colorDictionary == null)
                colorDictionary = new Dictionary<string, bool>();
            playerControllerScript = GetComponent<PlayerController>();
            playerBlinkScript = GetComponent<Blink>();
            playerAttackScript = GetComponent<PlayerAttack>();
            playerHealthScript = GetComponent<PlayerHealth>();
            playerInput = GetComponent<PlayerInput>();
            fadingtextEvent = new FadingTextEvent();
            resourceEvent = new ResourceUpdateEvent();
            craftingEvent = new CraftingEvent();
            craftingTable.SetActive(false);
            resourceArray = new int[] { copper, transistor, iron, currency };

        }

        private void Update()
        {
            if (!started)
            {
                isPlayerOne = playerAttackScript.IsPlayerOne();
                resourceEvent.isPlayerOne = isPlayerOne;
                fadingtextEvent.isPlayerOne = isPlayerOne;
                craftingEvent.isPlayerOne = isPlayerOne;
                craftingEvent.activate = false;
                EventSystem.Current.FireEvent(craftingEvent);
                UpdateResources();

                defaultColor = isPlayerOne ? new Color(0.3f, 0.9f, 0.3f, 1f) : new Color(0.3f, 0.3f, 0.9f, 1f);
                started = true;
            }
        }

        public void DropLoot()
        {
            /*
            dropOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1f, UnityEngine.Random.Range(-1f, 1f));
            //dropOffset = Random.onUnitSphere * 2f;
            dropOffset.y = Mathf.Abs(dropOffset.y);
            */
 
                for (int i = 0; i < resourceArray[i] / 2; i++)
                {
              //      drop = dropTable[index];
              //      GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
              //      Destroy(loot, 15f);
                }
            
            BisectResources();
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
                        if (hit.transform.tag == "CraftingTable")
                        {
                            EnterCraftingUI();
                        }
                        else if (hit.transform.tag == "Generator")
                        {
                            GeneratorEvent generator = hit.transform.GetComponent<GeneratorEvent>();
                            generator.StartGenerator();
                        }
                        else if (hit.transform.tag == "Exit")
                        {
                            OpenExit exit = hit.transform.GetComponentInParent<OpenExit>();
                            //exit.OpenDoor();
                        }
                    }
                }
            }
        }

        private void EnterCraftingUI()
        {
            isCrafting = !isCrafting;
            craftingEvent.successfulCraft = false;
            craftingEvent.isPlayerOne = isPlayerOne;

            if (!isCrafting)
            {
                craftingEvent.activate = false;
                playerInput.SwitchCurrentActionMap("Player");
            }
            else
            {
                craftingEvent.activate = true;
                playerInput.SwitchCurrentActionMap("Crafting");
            }
            EventSystem.Current.FireEvent(craftingEvent);
        }

        //%--------------------------------Crafts & upgrades---------------------------------%

        public void CraftBullet(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (TryCraftRecipe(bulletRecipe))
                {
                    playerAttackScript.CraftAmmoBox();
                    AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.ammoCraft, gameObject);
                    fadingtextEvent.text = "Ammo Box Crafted";
                    EventSystem.Current.FireEvent(fadingtextEvent);
                }
                else
                {
                    Debug.Log("Carrying max bullets!");
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
                        AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.batteryCraft, gameObject);
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

        public void CraftUpgradedProjectileWeapon()
        {
            if (TryCraftRecipe(UpgradedProjectileWeaponRecipe))
            {
                playerAttackScript.UpgradeProjectileWeapon();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Revolver damage Upgraded";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftCrittableRevolver()
        {
            if (TryCraftRecipe(RevolverCritRecipe))
            {
                playerAttackScript.RevolverCritUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Revolver Crit Enabled";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftUpgradedRevolverAmmo()
        {
            if (TryCraftRecipe(largeMagazineRecipe))
            {
                playerAttackScript.RevolverMagazineUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Revolver Ammo Upgraded";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }


        public void CraftReducedBeamCharge()
        {
            if (TryCraftRecipe(laserbeamChargeRecipe))
            {
                playerAttackScript.LaserChargeRateUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Lasergun Upgraded";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftIncreaseBeamWidth()
        {
            if (TryCraftRecipe(laserbeamWidthRecipe))
            {
                playerAttackScript.LaserBeamWidthUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Lasergun Upgraded";
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
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Lasergun Upgraded";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        //%-----------------------------------Colors------------------------------------%

        public void CraftMaterialColorCyan()
        {
            if (TryCraftRecipe(cyanRecipe))
            {
                playerHealthScript.ChooseMaterialColor(cyanColor);
                playerHealthScript.SetDefaultStats();
                playerAttackScript.UpgradeLaserWeapon();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color Cyan Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }
        public void CraftMaterialColorYellow()
        {
            if (TryCraftRecipe(yellowRecipe))
            {
                playerHealthScript.ChooseMaterialColor(Color.yellow);
                playerHealthScript.SetDefaultStats();
                playerControllerScript.MovementSpeedUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color Yellow Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }
        public void CraftMaterialColorWhite()
        {
            if (TryCraftRecipe(whiteRecipe))
            {
                playerHealthScript.ChooseMaterialColor(whiteColor);
                playerHealthScript.SetDefaultStats();
                playerBlinkScript.DecreaseBlinkCooldown();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color White Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }
        public void CraftMaterialColorMagenta()
        {
            if (TryCraftRecipe(magentaRecipe))
            {
                playerHealthScript.ChooseMaterialColor(magentaColor);
                playerHealthScript.SetDefaultStats();
                playerAttackScript.UpgradeLaserWeapon();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color Magenta Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorGreen()
        {
            if (TryCraftRecipe(greenRecipe))
            {
                playerHealthScript.ChooseMaterialColor(greenColor);
                playerHealthScript.SetDefaultStats();
                playerControllerScript.MovementSpeedUpgrade();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color Green Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftMaterialColorBlack()
        {
            if (TryCraftRecipe(blackRecipe))
            {
                playerHealthScript.ChooseMaterialColor(blackColor);
                playerHealthScript.SetDefaultStats();
                playerBlinkScript.DecreaseBlinkCooldown();
                AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
                fadingtextEvent.text = "Color Black Crafted";
            }
            else
                fadingtextEvent.text = "Not Enough Resources";
            EventSystem.Current.FireEvent(fadingtextEvent);
        }

        public void CraftDefaultColor()
        {
            playerHealthScript.ChooseMaterialColor(defaultColor);
            playerHealthScript.SetDefaultStats();
            AudioController.instance.PlayOneShotAttatched(AudioController.instance.craftingSound.craftTable, gameObject);
            fadingtextEvent.text = "Default Color Crafted";
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
                DisableButton();
                return true;
            }
            return false;
        }

        public void DisableButton()
        {
            craftingEvent.successfulCraft = true;
            craftingEvent.isPlayerOne = isPlayerOne;
            EventSystem.Current.FireEvent(craftingEvent);
        }

        public void BisectResources()
        {
            copper /= 2;
            iron /= 2;
            transistor /= 2;
            currency /= 2;
            UpdateResources();
        }
    }
}