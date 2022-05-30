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
    [SerializeField] private float impactForce = 40f;
    [SerializeField] private float hitForce = 10;
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
            else if (hitInfo.transform.tag == "BreakableObject")
            {
                BreakableObject breakable = hitInfo.transform.GetComponent<BreakableObject>();
                breakable.DropBoxLoot();
            }
            else if (hitInfo.transform.tag == "Enemy" || hitInfo.transform.tag == "Player")
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>();
                hitInfo.transform.rotation = new Quaternion(Mathf.PingPong(Time.deltaTime * hitForce, -30), hitInfo.transform.rotation.y, hitInfo.transform.rotation.z, 0);

                if (damageable != null)
                {
                    if (hitInfo.transform.tag == "Player")
                        damageable.TakeDamage(damage * .5f);
                    else
                        damageable.TakeDamage(damage);
                }

                Ricochet();
            }
            
            //else if (1 << hitInfo.collider.gameObject.layer == enemyLayerMask)
            //{
            //    hit = true;
            //    IDamageable target = hitInfo.transform.GetComponent<IDamageable>();
            //
            //    DamageEnemy(target);
            //    if (hitInfo.rigidbody != null)
            //    {
            //        hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
            //    }
            //    Ricochet();
            //}
            //else if (hitInfo.transform.tag == "Player")
            //{
            //    hit = true;
            //    IDamageable target = hitInfo.transform.GetComponent<IDamageable>();
            //
            //    DamageEnemy(target);
            //    if (hitInfo.rigidbody != null)
            //    {
            //        hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
            //    }
            //    Ricochet();
            //}
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

