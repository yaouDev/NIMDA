using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyDrop : MonoBehaviour
{
    [System.NonSerialized] public bool isInitialized = false;
    [SerializeField] private Vector3 dropOffset;
    //The variable that represents the enemy health.
    private float health;
    private GameObject drop;

    void Update()
    {
        if(health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Drop();
    }

    private void Drop()
    {
        Vector3 pos = transform.position;
        GameObject loot = Instantiate(drop, pos + dropOffset, Quaternion.identity);
        loot.SetActive(true);
        Destroy(loot, 15f);
       
    }
}