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
                Crafting crafting = other.transform.GetComponent<Crafting>();
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
                    break;
                case (PickUp.Copper):
                    crafting.copper++;
                    break;
                case (PickUp.Transistor):
                    crafting.transistor++;
                    break;
            }
            UpdateRes();

            void UpdateRes()
            {
                UpdateResources.c = crafting.copper;
                UpdateResources.t = crafting.transistor;
                UpdateResources.i = crafting.iron;
                EventSystem.Current.FireEvent(UpdateResources);
            }
        }
    }
}

