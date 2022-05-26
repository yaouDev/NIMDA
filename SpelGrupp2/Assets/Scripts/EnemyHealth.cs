using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IPoolable {
    [SerializeField] private GameObject[] dropList;
    [SerializeField] private float healthRestoreRate;
    [SerializeField] private GameObject firePoint;
    [SerializeField][Range(0, 2000)] private int fullHealth = 100;
    [SerializeField] private float hitForce = 10;
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
    [SerializeField] private int dropMin;
    [SerializeField] private int dropMax;
    private EnemyShield shield;
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
        shield = GetComponentInChildren<EnemyShield>();
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
    /*     public GameObject GetFirePoint() {
            return firePoint;
        } */

    public Vector3 FirePoint {
        get { return firePoint.transform.position; }
    }



    public void Die() {
        enemySpawnController.reduceSpawnCount(1);
        DropLoot();
        AudioController.instance.PlayOneShotAttatched(AudioController.instance.enemySound.death, gameObject);
        Destroy(gameObject);
        //ObjectPool.Instance.ReturnToPool(objectPoolTag, gameObject);
    }
    public void DieNoLoot() {
        enemySpawnController.reduceSpawnCount(1);
        Destroy(gameObject);
        //ObjectPool.Instance.ReturnToPool(objectPoolTag, gameObject);
    }

    /*  public float GetCurrentHealth() {
          return currHealth;
      }*/

    public void DropLoot() {

        int dropAmount = Random.Range(dropMin, dropMax);
        for (int i = 0; i < dropAmount; i++) {
            dropOffset = new Vector3(Random.Range(-.3f, .3f), 1f, Random.Range(-.3f, .3f));
            int dropRoll = Random.Range(0, 100);
            if (dropRoll <= ironRange) {
                drop = dropList[0];
            } else if (dropRoll <= copperRange) {
                drop = dropList[1];
            } else if (dropRoll <= transitorRange) {
                drop = dropList[2];
            }
            //int item = Random.Range(0, dropList.Length);
            //drop = dropList[item];
            GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
            //loot.transform.parent = null;
            //loot.SetActive(true);
            Destroy(loot, 15f);
        }
    }

    public void TakeDamage(float damage) {
        currentHealth -= damage;
        Instantiate(AIData.Instance.EnemyHitParticles, transform.position, Quaternion.identity);
        //BELOW USES FIND! BAD BAD BAD! GET A REAL REFERENCE!!! // -- fixed
        AudioController.instance.PlayOneShot(AudioController.instance.enemySound.hurt, playersPos.position);
        //Debug.Log(currentHealth);
        agent.transform.rotation = new Quaternion(Mathf.PingPong(Time.deltaTime * hitForce, -30), agent.transform.rotation.y, agent.transform.rotation.z, 0);

    }

    public void OnSpawn() {
        CurrentHealth = fullHealth;
        if (shield != null) {
            shield.CurrentHealth = shield.FullHealth;
            shield.gameObject.SetActive(true);
        }
        if (AIData.Instance.GetShotRequirement(agent) != -1) {
            AIData.Instance.ResetShotsFired(agent);
        }
        agent.Destination = Vector3.zero + new Vector3(1, 0, 0);
    }

}
