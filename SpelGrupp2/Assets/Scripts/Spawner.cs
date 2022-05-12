using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnThis; 
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int dayMaxSpawnCount;
    [SerializeField] private int nightMaxSpawnCount;
    [SerializeField] private float lifeTime = 10f;
    public int spawnCount;
    

    [SerializeField] private Transform spawnPos;
    [SerializeField] private DayNightSystem dns;



    private void Start()
    {
        StartCoroutine(SpawnObject());
    }

    private void Update()
    {
        if (dns.isDay == true)
        {
            maxSpawnCount = dayMaxSpawnCount;
        }
        else
        {
            maxSpawnCount = nightMaxSpawnCount;
        }
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                spawnCount--;
                Detstruction();
            }
        }
    }
    private void Detstruction()
    {
        Destroy(spawnThis.gameObject);
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            if (spawnCount < maxSpawnCount)
            {
                Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
                spawnCount += 1;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

}
