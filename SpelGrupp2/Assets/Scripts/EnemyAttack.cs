using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Transform target;

    [Range(0f, 10f)]private float combatDist;


    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, target.position) <= combatDist)
        {
            Attack();
        }   
    }

    private void Attack()
    {
        //transform.
    }
}
