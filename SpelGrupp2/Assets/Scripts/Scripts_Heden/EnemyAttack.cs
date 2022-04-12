using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject target;
    private Transform targetLocation;
    [Range(0f, 10f)] private float combatDist;
    [SerializeField] LayerMask layerMask;

    void Awake()
    {
        targetLocation = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetLocation.position) <= combatDist)
        {
            TurnToTarget();
            Attack();
        }
    }

    private void Attack()
    {
        Physics.Raycast(transform.position + transform.forward + Vector3.up, transform.forward, out RaycastHit hitInfo, 30.0f);
        //Debug.Log(hitInfo.collider.transform.name);
        if (hitInfo.collider != null)
        {
            PlayerController player = hitInfo.transform.GetComponent<PlayerController>();
            //player.TakeDamage();
        }
    }

    private void TurnToTarget()
    {
        transform.LookAt(targetLocation);
    }
}
