using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject spawnThis;
    [SerializeField] private int maxSpawnCount;
    private int spawnCount;
    private GameObject[] players;
    private GameObject[] spawnLocations;
    private List<GameObject> nearbySpawners = new List<GameObject>();
    private int index;
    private GameObject activeSpawner;
    private Transform spawnPos;
    [SerializeField] private float spawnCooldown;
    [SerializeField] private int spawnDistanceMax;
    [SerializeField] private int spawnDistanceMin;

    // Start is called before the first frame update
    void Start()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(SpawnObject());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnObject()
    {
        while (true)
        {
            if (spawnLocations.Length == 0)
            {
                spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");
            }

            for (int i = 0; i < spawnLocations.Length; i++)
            {
                if ((Vector3.Distance(players[0].transform.position, spawnLocations[i].transform.position) < spawnDistanceMax) || (Vector3.Distance(players[1].transform.position, spawnLocations[i].transform.position) < spawnDistanceMax))
                {
                    nearbySpawners.Add(spawnLocations[i]);
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
