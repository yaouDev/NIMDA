using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;
    public float minModifier = 7, maxModifier = 11;
    private bool isFollowing;
    private Vector3 velocity = Vector3.zero;

    public void StartFollowing(Transform player)
    {
        Target = player;
        isFollowing = true;
    }

    void Update()
    {
        if(isFollowing)
            transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref velocity, Time.deltaTime * Random.Range(minModifier, maxModifier));
    }
}
