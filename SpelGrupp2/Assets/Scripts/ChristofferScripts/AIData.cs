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
    [SerializeField] private ParticleSystem explosionParticles; 
    [SerializeField] private ParticleSystem enemyHitParticles;
    [SerializeField] private ParticleSystem blueShieldHitParticles;
    [SerializeField] private ParticleSystem greenShieldHitParticles;
    [SerializeField] private ParticleSystem purpleShieldHitParticles;
    [SerializeField] private ParticleSystem yellowShieldHitParticles;
    [SerializeField] private ParticleSystem enemyMuzzleflash;
    [SerializeField] private ParticleSystem fireParticles;

    private ConcurrentDictionary<Vector2Int, ConcurrentDictionary<Vector3, byte>>
        potentialCoverSpots = new ConcurrentDictionary<Vector2Int, ConcurrentDictionary<Vector3, 
        byte>>();
    private void Start() {
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
    public ParticleSystem ExplosionParticles
    {
        get { return explosionParticles; }
    }
    public ParticleSystem EnemyHitParticles {
        get { return enemyHitParticles; }
    }
    public ParticleSystem BlueShieldHitParticles
    {
        get { return blueShieldHitParticles; }
    }
    public ParticleSystem GreenShieldHitParticles
    {
        get { return greenShieldHitParticles; }
    }
    public ParticleSystem PurpleShieldHitParticles
    {
        get { return purpleShieldHitParticles; }
    }
    public ParticleSystem YellowShieldHitParticles
    {
        get { return yellowShieldHitParticles; }
    }
    public ParticleSystem EnemyMuzzleflash {
        get { return enemyMuzzleflash; }
    }
    public ParticleSystem FireParticles {
        get { return fireParticles; }
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

    public void ResetShotsFired(AI_Controller agent) {
        if (shotsToFireAndFired.ContainsKey(agent)) shotsToFireAndFired[agent].Value = 0;
    }

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
