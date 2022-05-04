using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenerator : MonoBehaviour
{
    private static int width = 200;
    private static int height = 200;

    private Dictionary<Vector2Int, Module> instantiatedModules = new Dictionary<Vector2Int, Module>();
    private List<Vector2Int> currentModules = new List<Vector2Int>();

    [SerializeField] private Transform player;

    private int maxVisibleChunkWidth = 2;
    private int maxDeleteWidth;
    private float counter = 0.0f;

    private uint[,] graph;

    public GameObject[] Modules;

    public static Queue<GameObject>[] modulePool = new Queue<GameObject>[16];
    
    private class Module
    {
        private WorldGenerator worldGenerator;
        public bool active;
        public Vector2Int pos;
        public GameObject m;
        private uint module;

        public Module(Vector2Int pos, WorldGenerator worldGenerator, uint module)
        {
            this.pos = pos;
            this.worldGenerator = worldGenerator;
            this.module = module;
            ActivateModule();
        }

        public void ActivateModule()
        {
            if (active)
                return;
            active = true;
            
            if (m == null)
            {
                if (WorldGenerator.modulePool[(int) module].Count > 0)
                {
                    m = WorldGenerator.modulePool[(int) module].Dequeue();
                    m.transform.position = new Vector3(pos.x * 50, 0, pos.y * 50);
                }
                else
                {
                    m = Instantiate(worldGenerator.Modules[(int)module], new Vector3(pos.x * 50, 0, pos.y * 50), Quaternion.identity, worldGenerator.transform);
                }
            }
            m.SetActive(true); // TODO [Patrik] move into pool deque, ~line 55
            
            // TODO [Patrik]
            // Callback function, send to add to pathfinding, send: Vec2Int, uint 
        }

        public void DeactivateModule()
        {
            m.SetActive(false);
            active = false;
            WorldGenerator.modulePool[(int) module].Enqueue(m);
            // TODO [Patrik]
            // Killbox for enemies on worldgenerator
            // Callback function, send to remove from pathfinding, send: Vec2Int, uint 
            // Deactivate this module, return to pool of this particular module
        }
    }

    private void Start()
    {
        for (int i = 0; i < modulePool.Length; i++)
        {
            modulePool[i] = new Queue<GameObject>();
        }
        
        graph = GetComponent<ProceduralWorldGeneration>().Get();
    }
    
    private void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;
        if (true)//counter > .1f)
        {
            counter = 0.0f;
            AddChunks();
            RemoveChunks();
        }
    }

    private void RemoveChunks()
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        for (int i = 0; i < currentModules.Count; i++)
        {
            Vector2Int pos = new Vector2Int((int) (player.position.x - 50) / 50, (int) (player.position.z - 50) / 50);
            if (currentModules[i].x > pos.x + maxVisibleChunkWidth ||
                currentModules[i].y > pos.y + maxVisibleChunkWidth ||
                currentModules[i].x < pos.x ||
                currentModules[i].y < pos.y )
            {
                instantiatedModules[currentModules[i]].DeactivateModule();
                toRemove.Add(currentModules[i]);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            if (currentModules.Contains(toRemove[i]))
            {
                if (instantiatedModules.ContainsKey(toRemove[i]))
                {
                    instantiatedModules.Remove(toRemove[i]);
                }
                currentModules.Remove(toRemove[i]);
            }
        }
    }

    private void AddChunks()
    {
        for (int y = -maxVisibleChunkWidth; y <= maxVisibleChunkWidth; y++)
        {
            for (int x = -maxVisibleChunkWidth; x <= maxVisibleChunkWidth; x++)
            {
                Vector2Int pos = new Vector2Int((int) ((player.position.x - 50) / 50 + x), (int) ((player.position.z - 50) / 50 + y));
                if (instantiatedModules.ContainsKey(pos))
                {
                    instantiatedModules[pos].ActivateModule();
                }
                else if (x > 0 && y > 0 && x < graph.GetLength(0) && y < graph.GetLength(1))
                {
                    Module c = new Module(pos, this, graph[pos.x, pos.y]);
                    instantiatedModules.Add(pos, c);
                    if (!currentModules.Contains(pos))
                        currentModules.Add(pos);
                }
            }
        }
    }

    public void DestroyModule(GameObject go)
    {
        Destroy(go);
    }
}