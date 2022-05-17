using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform player;
    private AI_Controller ai;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponentInParent<AI_Controller>();
        //player = FindObjectOfType<PlayerHealth>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (ai != null)
            transform.LookAt(ai.ClosestPlayer);
    }
}
