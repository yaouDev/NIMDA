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
        private ResourceUpdateEvent UpdateResources;
        private void Awake()
        {
            UpdateResources = new ResourceUpdateEvent();
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

                UpdateResources.isPlayerOne = crafting.IsPlayerOne();
                UpdateResources.c = crafting.copper;
                UpdateResources.t = crafting.transistor;
                UpdateResources.i = crafting.iron;
                //Debug.Log("Copper: " + UpdateResources.c + ". Transistor: " + UpdateResources.t + ". Iron: " + UpdateResources.i);

                EventSystem.Current.FireEvent(UpdateResources);
            }
        }
    }
}

