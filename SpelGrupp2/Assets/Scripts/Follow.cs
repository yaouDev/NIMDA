using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Follow : MonoBehaviour
{
    [HideInInspector] public CallbackSystem.Crafting[] targets;
    [HideInInspector] public CallbackSystem.Crafting target;
    public float minModifier = 7, maxModifier = 11;
    private bool targetFound, kickoff;
    private Vector3 velocity = Vector3.zero, offset = Vector3.up/2;

    [SerializeField] private EventReference pickupSound;
    private AudioController ac;
    private bool isSoundPlayed;

    private void Start()
    {
        targets = FindObjectsOfType<CallbackSystem.Crafting>();

        ac = AudioController.instance;
    }
    
    public void StartFollowing(GameObject player)
    {
        targetFound = true;
    }
    

    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);

        if(!targetFound)
        {
            if ((targets[0].transform.position - transform.position).sqrMagnitude < 30 * 3)
            {
                target = targets[0];
                targetFound = true;
            }
            if ((targets[1].transform.position - transform.position).sqrMagnitude < 30 * 3)
            {
                target = targets[1];
                targetFound = true;
            }
        }
        
        
        if (targetFound)
        {
            if (!kickoff)
            {
                transform.position += offset;
                kickoff = true;
            }
            if (!isSoundPlayed)
            {
                ac.PlayOneShotAttatched(pickupSound, gameObject);
                isSoundPlayed = true;
            }
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position, 
                ref velocity, Time.deltaTime * Random.Range(minModifier, maxModifier));
        }
    }
}
