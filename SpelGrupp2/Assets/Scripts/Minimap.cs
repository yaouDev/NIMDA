using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private uint[,] graph = {
        {15,15,15,15,15,15,15,15,15,15,15,15,15,15},
        {15,15,15,15,15,15,17,15,15,15,15,15,15,15},
        {15,15,15,15, 9,10,16,15,15,15,15,15,15,15},
        {15,15,15,11, 1, 6, 3,15,15,15,15,15,15,15},
        {15,15,15, 5, 0,12, 6,15,15,15,15,15,15,15},
        {15,15,15,15, 3,15,15,15,15,15,15,15,15,15},
        {15,15,15,15,16,15,15,15,15,15,15,15,15,15},
        {15,15,11, 9, 6,15,15,15,15,15,15,15,15,15},
        {15,15, 1, 2,15,15,15,15,15,15,15,15,15,15},
        {15, 13,0, 4,14,15,15,15,15,15,15,15,15,15},
        {15,15, 5,12,10,15,15,15,15,15,15,15,15,15},
        {15,15,15, 9, 6,15,15,15,15,15,15,15,15,15},
        {15,15,15, 7,15,15,15,15,15,15,15,15,15,15},
        {15,15,15,15,15,15,15,15,15,15,15,15,15,15}
    };

    private PlayerHealth[] players;
    
    private WorldGenerator worldGenerator;
    private ProceduralWorldGeneration procGenWorldGenerator;
    //private uint[,] graph;
    [SerializeField] private Sprite[] moduleImages;
    [SerializeField] private Image[] playerImages;
    [SerializeField] private Image[] map;

    private void Start()
    {
        players = FindObjectsOfType<PlayerHealth>();
        //procGenWorldGenerator = FindObjectOfType<ProceduralWorldGeneration>();
        //if (procGenWorldGenerator != null)
        //{
            //graph = procGenWorldGenerator.Get();
            for (int y = 0; y < graph.GetLength(1); y++)
            {
                for (int x = 0; x < graph.GetLength(0); x++)
                {
                    RevealWorldMapCoordinate(x, y);
                }
            }
        //}
    }

    private void RevealWorldMapCoordinate(int x, int y)
    {
        // 14 height
        // 8 wide
        int index = IndexFromCoord(x, y);
        uint moduleType = graph[x, y];
        map[index].sprite = moduleImages[graph[x, y]];
        
        if (moduleType == 15)
            map[index].color = Color.clear;
        else if (moduleType == 16)
            map[index].color = Color.yellow;
        else if (moduleType == 17)
            map[index].color = Color.red;
        
        int IndexFromCoord(int x, int y)
        {
            int index = x * 14 + y; 
            return index;
        }
    }

    private void Update()
    {
        UpdatePlayerPositions();
    }

    private float of = 6.3f;
    public void UpdatePlayerPositions()
    {
        Vector2 offset = new Vector3(28, -100);
        for (int player = 0; player < players.Length; player++)
        {
            Vector2 pos = new Vector3(players[player].transform.position.x, players[player].transform.position.z);
            //Debug.Log($"{players[player].transform.position}");
            pos.x /= of;
            pos.y /= of;
            playerImages[player].rectTransform.anchoredPosition = pos + offset;
        }
    }
}
