using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    [SerializeField] private GameObject[] dropList;
    [SerializeField] private float startingHelath; 
    [SerializeField] private float healthRestoreRate;

    private float healthBarLength;
    private GameObject drop;
    private Vector3 dropOffset = new Vector3(0f, 1f, 0f);

    //[Range(0, 3)] public int fullHealth;
    public float currentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, startingHelath); }
    }
   
    // Update is called once per frame
    private void Awake() {
        currentHealth = startingHelath;
    }
    void Update() {
        currentHealth += Time.deltaTime * healthRestoreRate;

        if (currentHealth <= 0) {
            Die();
        }
    }

    public float GetCurrentHealth(){
        return currentHealth;
    }

    public void TakeDamage() {
        --currentHealth;
    }

    public void Die() {
        DropLoot();
        Destroy(gameObject);

    }

    public void DropLoot() {
        int item = Random.Range(0, dropList.Length);
        Debug.Log(item);
        drop = dropList[item];
        GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
        loot.transform.parent = null;
        loot.SetActive(true);
        Destroy(loot, 15f);
    }
}
