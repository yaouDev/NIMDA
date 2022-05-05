using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] private GameObject[] dropList;
    [SerializeField] private float healthRestoreRate;
    [SerializeField] private float fleeHealthtreshold;
    [SerializeField] [Range(0, 100)] private int fullHealth;
    //public float currHealth;
    private float healthBarLength;
    private Vector3 dropOffset = new Vector3(0f, 1f, 0f);
    private GameObject drop;

    private float currentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, fullHealth); }
    }
    private void Awake() {
        currentHealth = fullHealth;
    }
    void Update() {

        currentHealth += Time.deltaTime * healthRestoreRate;

        if (currentHealth <= 0) {
            Die();
        }
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public float GetFleeHealthTreshold()
    {
        return fleeHealthtreshold;
    }

    public void TakeDamage() {
        --currentHealth;
    }

    public void Die() {
        DropLoot();
        Destroy(gameObject);

    }

  /*  public float GetCurrentHealth() {
        return currHealth;
    }*/

    public void DropLoot() {
        int item = Random.Range(0, dropList.Length);
        drop = dropList[item];
        GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
        loot.transform.parent = null;
        loot.SetActive(true);
        Destroy(loot, 15f);
    }
}
