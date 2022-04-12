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

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("item detected");
            PlayerController player = other.transform.GetComponent<PlayerController>();
            Debug.Log("player identified");
            pickUpDrop(player.crafting);
            Debug.Log("resource assigned");
            Destroy(gameObject);
            Debug.Log("item destroyed");

        }
    }
    private void pickUpDrop(Crafting crafting)
    {
        switch (pickUpType)
        {
            case (PickUp.Iron):
                crafting.iron++;
                Debug.Log("iron picked up");
                break;

            case (PickUp.Copper):
                crafting.copper++;
                Debug.Log("copper picked up");
                break;

            case (PickUp.Transistor):
                crafting.transistor++;
                Debug.Log("transistor picked up");
                break;
        }
    }
}
