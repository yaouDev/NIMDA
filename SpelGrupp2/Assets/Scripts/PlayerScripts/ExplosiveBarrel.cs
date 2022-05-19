using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [SerializeField] private float burnTime = 5f;
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private MeshRenderer barrelMesh;
    [SerializeField] private Transform burnSpot;
    [SerializeField] private LayerMask whatAreTargets;
    private ParticleSystem fire;

    private IDamageable damageable;
    private Collider[] colliders;


    public void StartExplosion()
    {

        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        fire = Instantiate(AIData.Instance.FireParticles, burnSpot.transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(burnTime);
        fire.Stop();
        barrelMesh.enabled = false;
        Instantiate(AIData.Instance.PulseAttackParticles, transform.position, Quaternion.identity);
        //explosionParticles.SetActive(true);
        colliders = Physics.OverlapSphere(transform.position, 4f, whatAreTargets);
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
        Instantiate(AIData.Instance.EnemyHitParticles, transform.position, Quaternion.identity);
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
            StartExplosion();
    }
}
