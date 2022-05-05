using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform orbitPivot;
    public float speed = 10f;
    public Vector3 axis;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(orbitPivot.position, axis, speed * Time.deltaTime);
    }
}
