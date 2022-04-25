using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnThis;
    public int spawnCount;
    [SerializeField] private int dayMaxSpawnCount;
    [SerializeField] private int nightMaxSpawnCount;
    public Transform spawnPos;
    private DayNightSystem dns;


    private void Update()
    {
        if (dns.isDay == true)
        {
            if (spawnCount < dayMaxSpawnCount)
            {
                Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
            }

        }
        else
        {
            if (spawnCount < nightMaxSpawnCount)
            {
                Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
            }
        }
    }
}
