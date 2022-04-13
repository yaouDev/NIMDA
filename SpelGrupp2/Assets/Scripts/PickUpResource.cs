using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpResource : MonoBehaviour
{
    private enum PickUp
    {
        Iron, Copper, Transistor
    }

    [SerializeField] private PickUp pickUpType;
    private bool pickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            PlayerController player = other.transform.GetComponent<PlayerController>();
            pickUpDrop(player.crafting);
            Destroy(gameObject);
        }
    }
    private void pickUpDrop(Crafting crafting)
    {
        if(!pickedUp)
        switch (pickUpType)
        {
            case (PickUp.Iron):
                crafting.iron++;
                Debug.Log("iron: " + crafting.iron);
                break;

            case (PickUp.Copper):
                crafting.copper++;
                Debug.Log("copper: " + crafting.copper);
                break;

            case (PickUp.Transistor):
                crafting.transistor++;
                Debug.Log("transistor: " + crafting.transistor);
                break;
        }
        pickedUp = true;
    }
}
