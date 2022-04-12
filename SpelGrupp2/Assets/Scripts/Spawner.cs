using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnThis;
    public float xPos;
    public float zPos;
    public int spawnCount;
    public int maxSpawnCount;


    private void Start()
    {
        StartCoroutine(SpawnDrop());
    }
    IEnumerator SpawnDrop()
    {
        while(spawnCount < maxSpawnCount)
        {
            xPos = Random.Range(-48, 48);
            zPos = Random.Range(-48, 48);
            Instantiate(spawnThis, new Vector3(xPos, 1.5f, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            spawnCount += 1;
        }
    }
}
