using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplosiveBullet : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask environmentLayerMask;
    [SerializeField] private float bulletSpeed = 150.0f;
    [SerializeField] private float impactForce = 40f;
    [SerializeField] private LayerMask whatAreTargets;
    private IDamageable damageable;
    private Collider[] colliders;
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
                Hit();
            }
            else if (hitInfo.transform.tag == "BreakableObject")
            {
                BreakableObject breakable = hitInfo.transform.GetComponent<BreakableObject>();
                breakable.DropBoxLoot();
            }
            else if (1 << hitInfo.collider.gameObject.layer == enemyLayerMask)
            {
                hit = true;
                IDamageable target = hitInfo.transform.GetComponent<IDamageable>();
                DamageEnemy(target);
                if (hitInfo.rigidbody != null)
                {
                    hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
                }
                Hit();
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

    private void Hit()
    {
        StartCoroutine(ExplosiveBullet());
    }

    private IEnumerator ExplosiveBullet()
    {
        Instantiate(AIData.Instance.PulseAttackParticles, transform.position, Quaternion.identity);
        colliders = Physics.OverlapSphere(transform.position, 4f, whatAreTargets);
        foreach (Collider coll in colliders)
        {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject"))
            {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null)
                    damageable.TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(0.5f);
        Stop();
    }

    private void Stop()
    {
        StopCoroutine(ExplosiveBullet());
        Destroy(gameObject);
    }
}

