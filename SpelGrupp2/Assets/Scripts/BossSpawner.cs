using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        
        // boss = GameObject.Find("Boss");
    }

    void OnTriggerEnter(Collider col)
    {
        playerCount++;
        if (col.gameObject.tag == "Player")
        {
            if (playerCount == 6)
            {
                boss.SetActive(true);
            }
        }
    }
}
