using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public static AIData instance;

    [SerializeField] private GameObject bullet;

    private Transform bestCoverSpot;
    private Material material;

    //[SerializeField] private GameObject muzzleflash;

    private void Start()
    {
        instance ??= this;
        material = GetComponent<MeshRenderer>().material;
    }

    public GameObject getBullet
    {
        get { return bullet; }
    }

    public void SetBestCoverSpot(Transform bestCoverSpot)
    {
        this.bestCoverSpot = bestCoverSpot;
    }
    public Transform GetBestCoverSpot()
    {
        return bestCoverSpot;
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }
    /* public GameObject getMuzzleflash
     {
         get { return muzzleflash; }
     }
 */
    //har en array av olika


}
