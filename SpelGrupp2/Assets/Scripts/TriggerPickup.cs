using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPickup : MonoBehaviour
{
    private Follow parent;
    private void Start()
    {
        parent = GetComponentInParent<Follow>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            parent.StartFollowing(collider.transform);
        }
    }
}
