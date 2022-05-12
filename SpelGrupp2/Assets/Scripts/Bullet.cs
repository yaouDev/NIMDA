using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask environmentLayerMask;
    [SerializeField] private ParticleSystem ricochetParticleSystem;
    [SerializeField] private float bulletSpeed = 150.0f;
    private bool hit;
    private float destroyTime = 5.0f;
    private float timeAlive = 0.0f;
    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > destroyTime)
            Destroy(gameObject);
        if (!hit)
            MoveBullet();
    }

    private void MoveBullet()
    {
        if (Physics.Raycast(
                transform.position,
                transform.forward,
                out RaycastHit hitInfo,
                bulletSpeed * Time.deltaTime,
                environmentLayerMask | enemyLayerMask))
        {
            transform.position += hitInfo.distance * transform.forward;

            if (1 << hitInfo.collider.gameObject.layer == environmentLayerMask)
            {
                Ricochet();
            }
            else if (1 << hitInfo.collider.gameObject.layer == enemyLayerMask)
            {
                hit = true;
                // TODO [Patrik] Update to call to IHealth Interface, thus we can shoot each other too <3
                IDamageable target = hitInfo.transform.GetComponent<IDamageable>();
                DamageEnemy(target);
                Ricochet();
            }
        }
        else
        {
            transform.position += bulletSpeed * Time.deltaTime * transform.forward;
        }
    }

    private void DamageEnemy(IDamageable target)
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }

    private void Ricochet()
    {
        // TODO [Patrik] Play ParticleSystem
        Destroy(gameObject);
    }
}
