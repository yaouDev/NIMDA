using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenerator : MonoBehaviour
{
    private static int width = 200;
    private static int height = 200;
    [SerializeField] 
    private GameObject module;

    private Dictionary<Vector2Int, Module> instantiatedModule = new Dictionary<Vector2Int, Module>();
    private List<Vector2Int> currentChunks = new List<Vector2Int>();

    private Transform player;

    private int maxVisibleChunkWidth = 4;
    private float counter = 0.0f;

    private uint[,] graph;

    public GameObject[] Modules;

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

            m = Instantiate(worldGenerator.Modules[module], (Vector2)(pos * 50), Quaternion.identity, worldGenerator.transform);
            // TODO [Patrik] place the correct module depending on worldGenerator
        }

        public void DeactivateModule()
        {
            // TODO [Patrik]
            // Callback function, send to remove from pathfinding
            // Deactivate this module, return to pool of this particular module
        }
    }

    private void Start()
    {
        graph = GetComponent<ProceduralWorldGeneration>().Get();

        for (int y = 0; y < maxVisibleChunkWidth; y++)
        {
            for (int x = 0; x < maxVisibleChunkWidth; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Module m = new Module(pos, this, graph[x,y]);
                instantiatedModule.Add(pos, m);
                currentChunks.Add(pos);
            }
        }
    }
    
    private void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;
        if (counter > .1f)
        {
            counter = 0.0f;
            RemoveChunks();
            AddChunks();
        }
    }

    private void RemoveChunks()
    {
        List<Vector2Int> toRemove = new List<Vector2Int>();

        for (int i = 0; i < currentChunks.Count; i++)
        {
            Vector2Int pos = new Vector2Int((int) player.position.x / 10, (int) player.position.z / 10);
            if (currentChunks[i].x > pos.x + maxVisibleChunkWidth ||
                currentChunks[i].y > pos.y + maxVisibleChunkWidth ||
                currentChunks[i].x < pos.x - maxVisibleChunkWidth ||
                currentChunks[i].y < pos.y - maxVisibleChunkWidth)
            {
                instantiatedModule[currentChunks[i]].DeactivateModule();
                toRemove.Add(currentChunks[i]);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            if (currentChunks.Contains(toRemove[i]))
                currentChunks.Remove(toRemove[i]);
        }
    }

    private void AddChunks()
    {
        for (int y = -maxVisibleChunkWidth + 1; y < maxVisibleChunkWidth - 1; y++)
        {
            for (int x = -maxVisibleChunkWidth + 1; x < maxVisibleChunkWidth - 1; x++)
            {
                Vector2Int pos = new Vector2Int((int) player.position.x / 10 + x, (int) player.position.z / 10 + y);
                if (instantiatedModule.ContainsKey(pos))
                {
                    instantiatedModule[pos].ActivateModule();
                }
                else
                {
                    Module c = new Module(pos, this, graph[x,y]);
                    instantiatedModule.Add(pos, c);
                    if (!currentChunks.Contains(pos))
                        currentChunks.Add(pos);
                }
            }
        }
    }
}