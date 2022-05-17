using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject Target;
    public float minModifier = 7, maxModifier = 11;
    private bool isFollowing, kickoff;
    private Vector3 velocity = Vector3.zero, offset = Vector3.up/2;

    public void StartFollowing(GameObject player)
    {
        Target = player;
        isFollowing = true;
    }

    void Update()
    {
        if (isFollowing)
        {
            if (!kickoff)
            {
                transform.position += offset;
            }
            transform.position = Vector3.SmoothDamp(transform.position, Target.transform.position, ref velocity, Time.deltaTime * Random.Range(minModifier, maxModifier));
        }
    }
}
