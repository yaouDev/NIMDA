using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private float force = 5f;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            PlayerHealth player = collider.gameObject.GetComponent<PlayerHealth>();
            player.inSafeZone = true;
            //player.batteryUI.batteryRecharge *= 2
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            PlayerHealth player = collider.gameObject.GetComponent<PlayerHealth>();
            //player.batteryUI.batteryRecharge /= 2;
            player.inSafeZone = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            EnemyAttack enemy = collision.gameObject.GetComponent<EnemyAttack>();
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;
            enemy.GetComponent<Rigidbody>().AddForce(dir * force);
            enemy.StunEnemy();
            
        }
    }
}
