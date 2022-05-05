using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour {
    public static AIData instance;

    [SerializeField] private GameObject bullet;

    private CallbackSystem.EventSystem eventSystem;
    private Transform bestCoverSpot;

    //private List<Transform> activeCovers = new List<Transform>(); 
    private List<Cover> activeCovers = new List<Cover>();
    //[SerializeField] private GameObject muzzleflash;

    private void Start() {
        eventSystem = FindObjectOfType<CallbackSystem.EventSystem>();

        eventSystem.RegisterListener<CallbackSystem.ModuleSpawnEvent>(LoadModule);
        eventSystem.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(UnLoadModule);

        instance ??= this;
    }

    public GameObject getBullet {
        get { return bullet; }
    }

    public void SetBestCoverSpot(Transform bestCoverSpot) {
        this.bestCoverSpot = bestCoverSpot;
    }
    public Transform GetBestCoverSpot() {

        return bestCoverSpot;
    }
    /* public List<Transform> GetActiveCovers()
     {
         return activeCovers;
     }*/
    public List<Cover> GetActiveCovers() {
        return activeCovers;
    }


    /* public GameObject getMuzzleflash
     {
         get { return muzzleflash; }
     }
 */
    //har en array av olika

    private void LoadModule(CallbackSystem.ModuleSpawnEvent moduleSpawnEvent)//ska ha ett event i paramatern
    {
        activeCovers.Add(moduleSpawnEvent.GameObject.GetComponentInChildren<Cover>());
    }

    private void UnLoadModule(CallbackSystem.ModuleDeSpawnEvent moduleDeSpawnEvent) //ska ha ett event i paramatern
    {
        /*        for (int i = 0; i < activeCovers.size; i++)
                {

                    activeCovers.RemoveRange(moduleDeSpawnEvent.GameObject.GetComponentInChildren<Cover>().GetCoverSpots());
                }
        */

    }


}
