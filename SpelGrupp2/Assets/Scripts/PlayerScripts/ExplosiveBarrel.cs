using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [SerializeField] private float burnTime = 5f;
    [SerializeField] private GameObject fireparticles;
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private MeshRenderer barrelMesh;
    [SerializeField] private Transform burnSpot;
    [SerializeField] private float health = 50f;



    public void StartExplosion()
    {

        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        fireparticles.transform.position = burnSpot.transform.position;
        fireparticles.SetActive(true);
        yield return new WaitForSeconds(burnTime);
        Destroy(fireparticles);
        fireparticles.SetActive(false);
        barrelMesh.enabled = false;
        explosionParticles.SetActive(true);
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
