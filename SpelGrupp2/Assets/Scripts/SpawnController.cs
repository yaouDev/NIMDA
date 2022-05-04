using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private GameObject spawnThis;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int dayMaxSpawnCount;
    [SerializeField] private int nightMaxSpawnCount;
    [SerializeField] private float lifeTime = 10f;
    public int spawnCount;
    [SerializeField] private GameObject[] spawners;
    private List<GameObject> nearbySpawners = new List<GameObject>();
    [SerializeField] private GameObject player2;
    private int index;
    private GameObject activeSpawner;
    private Transform spawnPos;
    [SerializeField] private float spawnCooldown;
    [SerializeField] private int spawnDistance;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnObject());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Active spawner is " + activeSpawner);
    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                if (Vector3.Distance(player2.transform.position, spawners[i].transform.position) < spawnDistance)
                {
                    nearbySpawners.Add(spawners[i]);
                }
            }
            if (nearbySpawners.Count > 0 && spawnCount < maxSpawnCount)
            {
                index = Random.Range(0, nearbySpawners.Count);
                activeSpawner = nearbySpawners[index];
                spawnPos = activeSpawner.transform;
                Instantiate(spawnThis, spawnPos.position, spawnPos.rotation);
                nearbySpawners.Clear();
                spawnCount += 1;
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }
}
