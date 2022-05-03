using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] private GameObject[] dropList;
    private Vector3 dropOffset = new Vector3(0f, 1f, 0f);
    [Range(0, 3)] public int fullHealth;
    public float currHealth;
    private float healthBarLength;
    private GameObject drop;

    // Update is called once per frame
    private void Awake() {
        currHealth = fullHealth;
    }
    void Update() {
        if (currHealth <= 0) {
            Die();
        }
    }

    public float GetCurrentHealth(){
        return currHealth;
    }

    public void TakeDamage() {
        --currHealth;
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
