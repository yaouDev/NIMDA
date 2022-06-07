using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PickUpResource : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private PlayerAttack playerAttack;
        private enum PickUp
        {
            Iron, Copper, Transistor, Bullet, Battery, Currency
        }

        [SerializeField] private PickUp pickUpType;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                playerAttack = other.GetComponent<PlayerAttack>();
                playerHealth = other.GetComponent<PlayerHealth>();
                Crafting crafting = other.gameObject.GetComponent<Crafting>();
                pickUpDrop(crafting);
            }
        }
        private void pickUpDrop(Crafting crafting)
        {
            if (playerHealth.Alive)
            {
                switch (pickUpType)
                {
                    case (PickUp.Iron):
                        crafting.iron++;
                        Destroy(gameObject);
                        break;

                    case (PickUp.Copper):
                        crafting.copper++;
                        Destroy(gameObject);
                        break;

                    case (PickUp.Transistor):
                        crafting.transistor++;
                        Destroy(gameObject);
                        break;

                    case (PickUp.Bullet):
                            playerAttack.CraftAmmoBox();
                            Destroy(gameObject);

                        break;
                    case (PickUp.Battery):
                        if (playerHealth.GetCurrentBatteryCount() < playerHealth.GetMaxBatteryCount())
                            playerHealth.IncreaseBattery();
                        else
                        {
                            playerHealth.SetCurrentHealth(playerHealth.GetMaxHealth());
                        }
                        Destroy(gameObject);
                        break;

                    case (PickUp.Currency):
                        crafting.currency++;
                        Destroy(gameObject);
                        break;
                }
                crafting.UpdateResources();
            }
        }
    }
}

