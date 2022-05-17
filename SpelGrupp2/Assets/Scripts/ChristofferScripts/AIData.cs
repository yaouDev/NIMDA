using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class AIData : MonoBehaviour {
    public static AIData Instance;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bossBigBullet;
    [SerializeField] private GameObject bossSmallBullet;
    [SerializeField] private ParticleSystem pulseAttackParticles;
    [SerializeField] private ParticleSystem enemyHitParticles;
    [SerializeField] private ParticleSystem enemyMuzzleflash;

    private CallbackSystem.EventSystem eventSystem;
    //private Transform bestCoverSpot;
    private Vector3 bestCoverSpot;



    //private List<Transform> activeCovers = new List<Transform>(); 
    private List<Cover> activeCovers = new List<Cover>();
    //[SerializeField] private GameObject muzzleflash;

    private void Start() {
        eventSystem = FindObjectOfType<CallbackSystem.EventSystem>();

        eventSystem.RegisterListener<CallbackSystem.ModuleSpawnEvent>(LoadModule);
        eventSystem.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(UnLoadModule);

        Instance ??= this;
    }

    public GameObject Bullet {
        get { return bullet; }
    }
    public GameObject BossBullet {
        get { return bossBigBullet; }
    }
    public GameObject SmallBullet {
        get { return bossSmallBullet; }
    }
    public ParticleSystem PulseAttackParticles {
        get { return pulseAttackParticles; }
    }
    public ParticleSystem EnemyHitParticles {
        get { return enemyHitParticles; }
    }
    public ParticleSystem EnemyMuzzleflash {
        get { return enemyMuzzleflash; }
    }

    /*     public void SetBestCoverSpot(Transform bestCoverSpot) {
            this.bestCoverSpot = bestCoverSpot;
        } */

    public void SetBestCoverSpot(Vector3 bestCoverSpot) {
        this.bestCoverSpot = bestCoverSpot;
    }


    /*     public Transform GetBestCoverSpot() {

            return bestCoverSpot;
        } */

    public Vector3 GetBestCoverSpot() {
        return bestCoverSpot;
    }
    /* public List<Transform> GetActiveCovers()
     {
         return activeCovers;
     }*/
    public List<Cover> GetActiveCovers() {
        return activeCovers;
    }


    /* public GameObject getMuzzleflash
     {
         get { return muzzleflash; }
     }
 */
    //har en array av olika

    private void LoadModule(CallbackSystem.ModuleSpawnEvent moduleSpawnEvent)//ska ha ett event i paramatern
    {
        activeCovers.Add(moduleSpawnEvent.GameObject.GetComponentInChildren<Cover>());
    }

    private void UnLoadModule(CallbackSystem.ModuleDeSpawnEvent moduleDeSpawnEvent) //ska ha ett event i paramatern
    {
        /*        for (int i = 0; i < activeCovers.size; i++)
                {

                    activeCovers.RemoveRange(moduleDeSpawnEvent.GameObject.GetComponentInChildren<Cover>().GetCoverSpots());
                }
        */

    }

    public class KeyValue<K, V> {
        public K Key { get; set; }
        public V Value { get; set; }

        public KeyValue() { }

        public KeyValue(K key, V val) {
            this.Key = key;
            this.Value = val;
        }
    }

    private Dictionary<AI_Controller, KeyValue<int, int>> shotsToFireAndFired = new Dictionary<AI_Controller, KeyValue<int, int>>();

    public int GetShotsFired(AI_Controller agent) {
        if (shotsToFireAndFired.ContainsKey(agent)) return shotsToFireAndFired[agent].Value;
        return 0;
    }

    public void SetShotRequirement(AI_Controller agent, int shotsToFire) {
        if (!shotsToFireAndFired.ContainsKey(agent)) shotsToFireAndFired.Add(agent, new KeyValue<int, int>());
        shotsToFireAndFired[agent].Key = shotsToFire;
        shotsToFireAndFired[agent].Value = 0;
    }

    public int GetShotRequirement(AI_Controller agent) {
        if (!shotsToFireAndFired.ContainsKey(agent)) return -1;
        return shotsToFireAndFired[agent].Key;
    }

    public void IncreaseShotsFired(AI_Controller agent) {
        if (shotsToFireAndFired.ContainsKey(agent)) shotsToFireAndFired[agent].Value = shotsToFireAndFired[agent].Value + 1;
    }

    private ConcurrentDictionary<Vector2Int, ConcurrentDictionary<Vector3, byte>> potentialCoverSpots = new ConcurrentDictionary<Vector2Int, ConcurrentDictionary<Vector3, byte>>();
    public void AddCoverSpot(Vector3 coverSpot) {
        Vector2Int modulePos = DynamicGraph.Instance.GetModulePosFromWorldPos(coverSpot);
        if (!potentialCoverSpots.ContainsKey(modulePos)) {
            potentialCoverSpots.TryAdd(modulePos, new ConcurrentDictionary<Vector3, byte>());
        }
        if (!potentialCoverSpots[modulePos].ContainsKey(coverSpot)) {
            potentialCoverSpots[modulePos].TryAdd(coverSpot, 0);
        }
    }

    public ConcurrentDictionary<Vector3, byte> GetNearbyCoverSpots(Vector2Int module) {
        if (potentialCoverSpots.ContainsKey(module)) return potentialCoverSpots[module];
        return null;
    }



}
