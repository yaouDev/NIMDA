using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable 
{
    [SerializeField] private GameObject[] dropList;
    [SerializeField] private float healthRestoreRate;
    //[SerializeField] private float healthThreashold;
    [SerializeField] private GameObject firePoint;
    [SerializeField][Range(0, 1000)] private int fullHealth;
    //public float currHealth;
    
    private Vector3 dropOffset = new Vector3(0f, 1f, 0f);
    private GameObject drop;
    private float currentHealth;
    [SerializeField] private int dropAmount;
    public float CurrentHealth {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, fullHealth); }
    }
    private void Awake() {
        CurrentHealth = fullHealth;
    }
    void Update() {

        if (CurrentHealth <= 0) {
            Die();
        } else {
            CurrentHealth += Time.deltaTime * healthRestoreRate;

        }
    }
    public float GetCurrentHealth() {
        return CurrentHealth;
    }
/*    public float GetFleeHealthTreshold() {
        return healthThreashold;
    }*/
    public GameObject GetFirePoint()
    {
        return firePoint;
    }

    public void Die() {
        DropLoot();
        Destroy(gameObject);

    }

    public void DropLoot() {
        for (int i = 0; i < dropAmount; i++)
        {
            int item = Random.Range(0, dropList.Length);
            //if (dropRoll > 0 && )
            drop = dropList[item];
            GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
            loot.transform.parent = null;
            loot.SetActive(true);
            Destroy(loot, 15f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
    }
}
