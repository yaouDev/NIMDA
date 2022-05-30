using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IPoolable {
    [SerializeField] private GameObject[] dropList;
    [SerializeField] private float healthRestoreRate;
    [SerializeField] private GameObject firePoint;
    [SerializeField][Range(0, 2000)] private int fullHealth = 100;
    [SerializeField] string objectPoolTag;
    //public float currHealth;
    private float healthBarLength;
    private Vector3 dropOffset;
    private GameObject drop;
    private float currentHealth;
    //[SerializeField] private int dropAmount;
    private EnemySpawnController enemySpawnController;

    [SerializeField] private int ironRange;
    [SerializeField] private int copperRange;
    [SerializeField] private int transitorRange;
    [SerializeField] private int currencyRange;
    [SerializeField] private int dropMin;
    [SerializeField] private int dropMax;
    private Transform playersPos;
    private AI_Controller agent;

    public float CurrentHealth {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, fullHealth); }
    }

    public float CurrentHealthPercentage {
        get {
            return currentHealth / fullHealth;
        }
    }

    private void Awake() {
        CurrentHealth = fullHealth;
        enemySpawnController = GameObject.Find("EnemySpawnController").GetComponent<EnemySpawnController>();
        playersPos = GameObject.Find("Players").transform;
        agent = GetComponent<AI_Controller>();
    }
    void Update() {

        if (CurrentHealth <= 0) {
            Die();
        } else {
            CurrentHealth += Time.deltaTime * healthRestoreRate;
        }
    }

    public float GetFullHealth() {
        return fullHealth;
    }

    public Vector3 FirePoint {
        get { return firePoint.transform.position; }
    }



    public void Die() {
        enemySpawnController.reduceSpawnCount(1);
        DropLoot();
        AudioController.instance.PlayOneShotAttatched(AudioController.instance.enemySound.death, gameObject);
        //Destroy(gameObject);
        ObjectPool.Instance.ReturnToPool(objectPoolTag, gameObject);
    }
    public void DieNoLoot() {
        enemySpawnController.reduceSpawnCount(1);
        //Destroy(gameObject);
        ObjectPool.Instance.ReturnToPool(objectPoolTag, gameObject);
    }

    public void DropLoot() {

        int dropAmount = Random.Range(dropMin, dropMax);
        for (int i = 0; i < dropAmount; i++) {
            dropOffset = new Vector3(Random.Range(-1.3f, 1.3f), 1f, Random.Range(-1.3f, 1.3f));
            int dropRoll = Random.Range(0, 100);
            if (dropRoll <= ironRange) {
                drop = dropList[0];
            } else if (dropRoll <= copperRange) {
                drop = dropList[1];
            } else if (dropRoll <= transitorRange) {
                drop = dropList[2];
            } else if(dropRoll <= currencyRange) {
                drop = dropList[3];
            }
            //int item = Random.Range(0, dropList.Length);
            //drop = dropList[item];
            GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
            loot.transform.parent = null;
            loot.SetActive(true);
            Destroy(loot, 15f);
        }
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        Instantiate(AIData.Instance.EnemyHitParticles, transform.position, Quaternion.identity);
        //BELOW USES FIND! BAD BAD BAD! GET A REAL REFERENCE!!! // -- fixed
        AudioController.instance.PlayOneShot(AudioController.instance.enemySound.hurt, playersPos.position);
        //Debug.Log(currentHealth);

    }

    public void OnSpawn() {
        CurrentHealth = fullHealth;
        agent.ResetAgent();
    }

}
