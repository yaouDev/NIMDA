using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Pickup-able objects require a rigidbody.
 * The name "PlayerPlaceHolder" refers to the players child object where picked-up object will appear.
 * Currently works on mouse objects.
 * */
public class PickUpableObject : MonoBehaviour
{
    [SerializeField] public Transform dest;
    public Rigidbody rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
        transform.position = dest.position;
        transform.parent = GameObject.Find("PlayerPlaceHolder").transform;
    }

    void OnMouseUp()
    {
        rb.useGravity = true;
        rb.freezeRotation = false;
        transform.parent = null;
    }
}
