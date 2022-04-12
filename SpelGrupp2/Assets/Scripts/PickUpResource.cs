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

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Equals("Player"))
        {
            PlayerController player = other.transform.GetComponent<PlayerController>();
            pickUpDrop(player.crafting);
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
    }
}
