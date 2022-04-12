using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyDrop : MonoBehaviour
{
    [System.NonSerialized] public bool isInitialized = false;
    [SerializeField] private GameObject[] dropList;
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

    public void Die()
    {
        Destroy(gameObject);
        Drop();
    }

    public void Drop()
    {
        int item = Random.Range(0, 2);
        drop = dropList[item];
        GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
        loot.SetActive(true);
        Destroy(loot, 15f);
    }
}