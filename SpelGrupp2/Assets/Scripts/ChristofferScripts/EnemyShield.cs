using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamageable
{
    [SerializeField] private float fullHealth = 50;
    private float currentHealth;

    public float CurrentHealth 
    {
        get { return currentHealth; }
    }

    private void Awake()
    {
        currentHealth = fullHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth < 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Instantiate(AIData.Instance.EnemyHitParticles, transform.position, Quaternion.identity);
    }

}
