using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject[] dropList;
    [SerializeField] private int ironRange;
    [SerializeField] private int copperRange;
    [SerializeField] private int transitorRange;
    [SerializeField] private int dropMin;
    [SerializeField] private int dropMax;
    [SerializeField] private Vector3 rotatationRate;
    private Vector3 dropOffset;
    private GameObject drop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotatationRate);
    }

    public void DropBoxLoot()
    {
        int dropAmount = Random.Range(dropMin, dropMax);
        for (int i = 0; i < dropAmount; i++)
        {
            dropOffset = new Vector3(Random.Range(-1.1f, 1.1f), 1f, Random.Range(-1.1f, 1.1f));
            int dropRoll = Random.Range(0, 100);
            if (dropRoll <= ironRange)
            {
                drop = dropList[0];
            }
            else if (dropRoll <= copperRange)
            {
                drop = dropList[1];
            }
            else if (dropRoll <= transitorRange)
            {
                drop = dropList[2];
            }
            GameObject loot = Instantiate(drop, transform.position + dropOffset, Quaternion.identity);
            loot.transform.parent = null;
            loot.SetActive(true);
            Destroy(loot, 15f);
        }
        Destroy(gameObject);
    }
}
