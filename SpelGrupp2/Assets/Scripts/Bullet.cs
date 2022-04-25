using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask environmentLayerMask;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float bulletSpeed = 20.0f;
    private bool hit;
    private void Update()
    {
        if (!hit)
            MoveBullet();
    }

    private void MoveBullet()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, bulletSpeed * Time.deltaTime, enemyLayerMask | environmentLayerMask);
        if (hitInfo.collider != null) {
            if (hitInfo.collider.gameObject.layer == enemyLayerMask)
            {
                EnemyHealth enemyHealth = hitInfo.transform.GetComponent<EnemyHealth>();
                DamageEnemy(enemyHealth);    
            }
            else if (hitInfo.collider.gameObject.layer == environmentLayerMask)
            {
                Ricochet();
            }
        }
        else
        {
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        }
    }

    private void DamageEnemy(EnemyHealth enemyHealth)
    {
        enemyHealth.TakeDamage();
    }

    private void Ricochet()
    {
        // TODO [Patrik] Play Particlesystem
        Destroy(gameObject);
    }
}
