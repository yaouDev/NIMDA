using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public static AIData instance;

    [SerializeField] private GameObject bullet;

    private CallbackSystem.EventSystem eventSystem;
    private Transform bestCoverSpot;
    private Material material;

    private List<Transform> activeCovers = new List<Transform>(); 

    //[SerializeField] private GameObject muzzleflash;

    private void Start()
    {
        FindObjectOfType<CallbackSystem.EventSystem>();

        //eventSystem.RegisterListener<LoadEvent>();

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

    private void LoadModule() //ska ha ett event i paramatern
    {
        //Covers.GetComponentInChildren<Cover>().coverSpots;
    }
    private void UnLoadModule() //ska ha ett event i paramatern
    {
        //Covers.GetComponentInChildren<Cover>().coverSpots;
    }


}
