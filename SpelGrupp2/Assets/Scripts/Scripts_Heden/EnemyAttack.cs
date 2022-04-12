using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject target;
    private Transform targetLocation;
    [Range(0.1f, 10f)]private float dmg;
    [Range(0f, 10f)]private float combatDist;
    [SerializeField] LayerMask layerMask;

    void Awake()
    {
        targetLocation = target.transform;
    }

    // Update is called once per frame
    void Update()
    { 
        if(Vector3.Distance(transform.position, targetLocation.position) <= combatDist)
        {
            Attack();
        }   
    }

    private void Attack()
    {
        RaycastHit hit;
        transform.LookAt(targetLocation);
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            /*
            if(hit.collider.GetComponent<Player>())
            Player player = hit.transform.GetComponent<Player>();
            player.TakeDmg(dmg);
            */
        }
    }
}
