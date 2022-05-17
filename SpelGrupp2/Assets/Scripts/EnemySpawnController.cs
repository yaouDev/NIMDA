using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [Header("What to Spawn")]
    [SerializeField] private GameObject[] spawnThis;
    [Header("Normal Spawn")]
    [SerializeField] private int maxSpawnCount = 25;
    [SerializeField] private float spawnCooldown = 5;
    [SerializeField] private int spawnDistanceMax = 35;
    [SerializeField] private int spawnDistanceMin = 5;
    [Header("Night Spawn")]
    [SerializeField] private int nightMaxSpawnCount = 30;
    [SerializeField] private float nightSpawnCooldown = 0.1f;
    [SerializeField] private int nightSpawnDistanceMax = 35;
    [SerializeField] private int nightSpawnDistanceMin = 5;
    [Header("Generator Spawn")]
    [SerializeField] private int genMaxSpawnCount = 40;
    [SerializeField] private float genSpawnCooldown = 0.1f;
    [SerializeField] private int genSpawnDistanceMax = 50;
    [SerializeField] private int genSpawnDistanceMin = 15;
    private int spawnCount;
    private int index;
    private float distanceP1;
    private float distanceP2;

    private GameObject[] players;
    private GameObject[] spawnLocations;
    private GameObject activeSpawner;
    private List<GameObject> nearbySpawners = new List<GameObject>();
    private Transform spawnPos;
    private DayNightSystem dayNightSystem;

    // Start is called before the first frame update
    void Start()
    {
        dayNightSystem = FindObjectOfType<DayNightSystem>();
        spawnLocations = GameObject.FindGameObjectsWithTag("SpawnLocation");
        players = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(SpawnObject());
    }
    private void FixedUpdate()
    {
        IsNight();
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
            // Debug.Log("hello");
            // Debug.Log(spawnLocations.Length);

            for (int i = 0; i < spawnLocations.Length; i++)
            {
                distanceP1 = Vector3.Distance(players[0].transform.position, spawnLocations[i].transform.position);
                distanceP2 = Vector3.Distance(players[1].transform.position, spawnLocations[i].transform.position);
                if ((distanceP1 < spawnDistanceMax && distanceP1 > spawnDistanceMin) || (distanceP2 < spawnDistanceMax && distanceP2 > spawnDistanceMin))
                {
                    nearbySpawners.Add(spawnLocations[i]);
                }
            }
            if (nearbySpawners.Count > 0 && spawnCount < maxSpawnCount)
            {
                index = Random.Range(0, nearbySpawners.Count);
                activeSpawner = nearbySpawners[index];
                spawnPos = activeSpawner.transform;
                Instantiate(spawnThis[Random.Range(0, 2)], spawnPos.position, spawnPos.rotation);
                nearbySpawners.Clear();
                spawnCount += 1;
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    public void reduceSpawnCount(int amount)
    {
        spawnCount -= amount;
    }

    public void GeneratorRunning(bool on)
    {
        if (on)
        {
            spawnCooldown = genSpawnCooldown;
            maxSpawnCount = genMaxSpawnCount;
            spawnDistanceMax = genSpawnDistanceMax;
            spawnDistanceMin = genSpawnDistanceMin;
        }
        if (!on)
        {
            spawnCooldown = 5;
            maxSpawnCount = 25;
            spawnDistanceMax = 35;
            spawnDistanceMin = 5;
        }
    }
    private void IsNight()
    {
        if (!dayNightSystem.Isday)
        {
            spawnCooldown = nightSpawnCooldown;
            maxSpawnCount = nightMaxSpawnCount;
            spawnDistanceMax = nightSpawnDistanceMax;
            spawnDistanceMin = nightSpawnDistanceMin;
        } else
        {
            spawnCooldown = 5;
            maxSpawnCount = 25;
            spawnDistanceMax = 35;
            spawnDistanceMin = 5;
        }
    }


}
