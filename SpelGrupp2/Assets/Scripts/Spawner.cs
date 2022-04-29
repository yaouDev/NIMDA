using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnThis;
    [SerializeField] private int spawnCount;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int dayMaxSpawnCount;
    [SerializeField] private int nightMaxSpawnCount;
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
    }

    IEnumerator SpawnObject()
    {

        while (spawnCount < maxSpawnCount)
        {
            Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
            yield return new WaitForSeconds(0.3f);
            spawnCount += 1;
        }

    }

}
