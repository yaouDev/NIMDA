using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PickUpResource : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private PlayerAttack playerAttack;
        [SerializeField] private GameObject parent;
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
                        Destroy(parent);
                        //Debug.Log("Picked up iron");
                        break;
                    case (PickUp.Copper):
                        crafting.copper++;
                        Destroy(parent);
                        //Debug.Log("Picked up copper");
                        break;
                    case (PickUp.Transistor):
                        crafting.transistor++;
                        Destroy(parent);
                        //Debug.Log("Picked up transistor");
                        break;
                    case (PickUp.Bullet):
                        if (playerAttack.ReturnBullets() < playerAttack.ReturnMaxBullets())
                            playerAttack.UpdateBulletCount(1);
                            Destroy(parent);
                        //Debug.Log("Picked up bullet");
                        break;
                    case (PickUp.Battery):
                        if (playerHealth.GetCurrentBatteryCount() < playerHealth.GetMaxBatteryCount())
                            playerHealth.IncreaseBattery();
                        else
                        {
                            playerHealth.SetCurrentHealth(playerHealth.GetMaxHealth());
                            //Debug.Log("Battery count too high. Max hp set");
                        }
                        Destroy(parent);
                        
                        //Debug.Log("Picked up Battery");
                        break;

                    case (PickUp.Currency):
                        crafting.currency++;
                        //Debug.Log("Picked up a bottlecap");
                        Destroy(parent);
                        break;
                }
                crafting.UpdateResources();
            }
        }
    }
}

