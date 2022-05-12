using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    [SerializeField] private GameObject boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.Find("Boss");
    }

    void OnTriggerEnter(Collision col)
    {
        boss.SetActive(true);
    }
}
