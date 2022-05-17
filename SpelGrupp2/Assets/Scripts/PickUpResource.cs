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
            Iron, Copper, Transistor, Bullet, Battery
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
                Destroy(parent);
            }
        }
        private void pickUpDrop(Crafting crafting)
        {
            switch (pickUpType)
            {
                case (PickUp.Iron):
                    crafting.iron++;
                    //Debug.Log("Picked up iron");
                    break;
                case (PickUp.Copper):
                    crafting.copper++;
                    //Debug.Log("Picked up copper");
                    break;
                case (PickUp.Transistor):
                    crafting.transistor++;
                    //Debug.Log("Picked up transistor");
                    break;
                case (PickUp.Bullet):
                    playerAttack.UpdateBulletCount(1);
                    //Debug.Log("Picked up bullet");
                    break;
                case (PickUp.Battery):
                    playerHealth.IncreaseBattery();
                    //Debug.Log("Picked up Battery");
                    break;
            }
            crafting.UpdateResources();
        }
    }
}

