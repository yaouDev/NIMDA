using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform Target;
    public float minModifier = 7, maxModifier = 11;
    private bool isFollowing = false;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(isFollowing)
        transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref velocity, Time.deltaTime * Random.Range(minModifier, maxModifier));
    }
}
