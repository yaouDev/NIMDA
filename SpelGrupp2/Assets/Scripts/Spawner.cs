using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnThis;
    public int spawnCount;
    public int maxSpawnCount;
    public Transform spawnPos;


    private void Update()
    {
        if(spawnCount < maxSpawnCount) 
        {
            Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
        }
    }
}
