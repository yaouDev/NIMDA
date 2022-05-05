using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class PickUpResource : MonoBehaviour
    {
        private enum PickUp
        {
            Iron, Copper, Transistor
        }

        [SerializeField] private PickUp pickUpType;
        private ResourceUpdateEvent resourceEvent;
        private void Awake()
        {
            resourceEvent = new ResourceUpdateEvent();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Crafting crafting = other.gameObject.GetComponent<Crafting>();
                pickUpDrop(crafting);
                Destroy(gameObject);
            }
        }
        private void pickUpDrop(Crafting crafting)
        {
            switch (pickUpType)
            {
                case (PickUp.Iron):
                    crafting.iron++;
                    Debug.Log("Picked up iron");
                    break;
                case (PickUp.Copper):
                    crafting.copper++;
                    Debug.Log("Picked up copper");
                    break;
                case (PickUp.Transistor):
                    crafting.transistor++;
                    Debug.Log("Picked up transistor");
                    break;
            }
            UpdateRes();

            void UpdateRes()
            {
            Debug.Log("Updated resources");

                resourceEvent.isPlayerOne = crafting.IsPlayerOne();
                resourceEvent.ammoChange = false;
                resourceEvent.c = crafting.copper;
                resourceEvent.t = crafting.transistor;
                resourceEvent.i = crafting.iron;
                Debug.Log("Copper: " + resourceEvent.c + ". Transistor: " + resourceEvent.t + ". Iron: " + resourceEvent.i);

                EventSystem.Current.FireEvent(resourceEvent);
            }
        }
    }
}

