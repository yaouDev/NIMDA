using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [SerializeField] private float burnTime = 5f;
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private GameObject fireparticles;
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private MeshRenderer barrelMesh;
    //[SerializeField] private Transform burnSpot;
    [SerializeField] private LayerMask whatAreTargets;

    private IDamageable damageable;
    private Collider[] colliders;


    public void StartExplosion()
    {

        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        fireparticles.transform.position = transform.position;
        fireparticles.SetActive(true);
        yield return new WaitForSeconds(burnTime);
        Destroy(fireparticles);
        fireparticles.SetActive(false);
        barrelMesh.enabled = false;
        explosionParticles.SetActive(true);
        colliders = Physics.OverlapSphere(transform.position, 4f, whatAreTargets);
        Debug.Log("PulseAttack");
        foreach (Collider coll in colliders)
        {
            if (coll.CompareTag("Player") || coll.CompareTag("BreakableObject"))
            {
                damageable = coll.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(damage);

                }

            }
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);

    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
            StartExplosion();
    }
}
