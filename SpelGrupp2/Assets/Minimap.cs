using System;
using System.Collections;
using System.Collections.Generic;
using CallbackSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private uint[,] graph = {
        {15,15,15,15,15,15,15,15,15,15,15,15,15,15},
        {15,15,15,17,15,15,15,15,15,15,15,15,15,15},
        {15,15,15,16,15,15,15,15,15,15,15,15,15,15},
        {15,15, 9, 4,14,15,15,15,15,15,15,15,15,15},
        {15,15, 1,10,11,15,15,15,15,15,15,15,15,15},
        {15,15, 5, 4, 2,15,15,15,15,15,15,15,15,15},
        {15,15,13, 8, 6,15,15,15,15,15,15,15,15,15},
        {15,15,15,16,15,15,15,15,15,15,15,15,15,15},
        {15,15,13, 0,10,15,15,15,15,15,15,15,15,15},
        {15,15, 9, 6, 7,15,15,15,15,15,15,15,15,15},
        {15,15, 1,12,14,15,15,15,15,15,15,15,15,15},
        {15,15, 5,10,15,15,15,15,15,15,15,15,15,15},
        {15,15,15, 7,15,15,15,15,15,15,15,15,15,15},
        {15,15,15,15,15,15,15,15,15,15,15,15,15,15}
    };

    private PlayerHealth[] players;
    private WorldGenerator worldGenerator;
    private ProceduralWorldGeneration procGenWorldGenerator;
    [SerializeField] public Sprite[] moduleImages;
    [SerializeField] private Image[] playerImages;
    [SerializeField] private Image[] map;
    private bool bossRoom;
    public float of = 6.3f;

    private void Start()
    {
        players = FindObjectsOfType<PlayerHealth>();
        
        for (int y = 0; y < graph.GetLength(1); y++)
        { 
            for (int x = 0; x < graph.GetLength(0); x++) 
            {
                RevealWorldMapCoordinate(x, y);
            }
        }
        
        EventSystem.Current.RegisterListener<BossRoomEvent>(BossRoomEvent);
        EventSystem.Current.RegisterListener<ChangeColorEvent>(ColorChange);
    }
    
    private void BossRoomEvent(BossRoomEvent bossRoomEvent)
    {
        bossRoom = bossRoomEvent.insideBossRoom;
        if (bossRoom)
            StartCoroutine(FadeAllMapImages());
    }

    private IEnumerator FadeAllMapImages()
    {
        float t = 1;
        while (t >= 0)
        {
            t -= Time.deltaTime;
            for (int image = 0; image < map.Length; image++)
            {
                Color color = map[image].color;
                color.a = Mathf.Min(color.a, Mathf.Lerp(0.0f, 1.0f, t));
                map[image].color = color;
            }

            for (int player = 0; player < playerImages.Length; player++)
            {
                Color color = playerImages[player].color;
                color.a = Mathf.Min(color.a, Mathf.Lerp(0.0f, 1.0f, t));
                playerImages[player].color = color;
            }

            yield return null;
        }
        for (int image = 0; image < map.Length; image++)
        {
            Color color = map[image].color;
            color.a = 0;
            map[image].color = color;
        }
        for (int player = 0; player < playerImages.Length; player++)
        {
            Color color = playerImages[player].color;
            color.a = 0;
            playerImages[player].color = color;
        }
    }

    private void ColorChange(ChangeColorEvent changeColorEvent)
    {
        playerImages[0].color = changeColorEvent.isPlayerOne ? changeColorEvent.color : playerImages[0].color;
        playerImages[1].color = !changeColorEvent.isPlayerOne ? changeColorEvent.color : playerImages[1].color;
    }

    private void RevealWorldMapCoordinate(int x, int y)
    {
        int index = IndexFromCoord(x, y);
        uint moduleType = graph[x, y];
        map[index].sprite = moduleImages[graph[x, y]];
        
        if (moduleType == 15)
            map[index].color = new Color(1,1,1,0);
        else if (moduleType == 16)
            map[index].color = Color.yellow;
        else if (moduleType == 17)
            map[index].color = Color.red;
    }

    private void Update()
    {
        if (!bossRoom)
            UpdatePlayerPositions();
    }

    private Vector2Int[] nineSquare = new Vector2Int[]
    {
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1,-1),
        new Vector2Int(-1, 0), new Vector2Int(0,0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1)
    };
    
    private void UpdatePlayerPositions()
    {
        Vector2 offset = new Vector3(28, -100);
        for (int player = 0; player < players.Length; player++)
        {
            Vector2 pos = new Vector3(players[player].transform.position.x, players[player].transform.position.z);
            
            //Debug.Log($"{players[player].transform.position}");
            pos.x /= of;
            pos.y /= of;
            playerImages[player].rectTransform.anchoredPosition = pos + offset;
            playerImages[player].rectTransform.rotation = Quaternion.Euler(0, 0, -players[player].transform.rotation.eulerAngles.y);
        }

        Vector2Int off = new Vector2Int(3, 2);
        for (int player = 0; player < players.Length && !bossRoom; player++)
        {
            Vector2Int playerPos = new Vector2Int((int)(players[player].transform.position.x / 50) + off.x, (int)(players[player].transform.position.z) / 50 + off.y);
            Vector2Int df = new Vector2Int(14 - playerPos.x, playerPos.y);
            for (int i = 0; i < nineSquare.Length; i++)
            {
                Color color = map[IndexFromCoord(df + nineSquare[i])].color;
                color.a += Time.deltaTime;
                map[ IndexFromCoord(df + nineSquare[i]) ].color = color;
            }
        }
        
        int IndexFromCoord(Vector2Int c)
        {
            int index = (14 - c.x) + (14 - c.y) * 14; 
            return index;
        }
    }
    
    public int IndexFromCoord(int x, int y)
    {
        int index = x * 14 + y; 
        return index;
    }
}