using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour {
    public static AIData Instance;

    [SerializeField] private GameObject bullet;

    private CallbackSystem.EventSystem eventSystem;
    //private Transform bestCoverSpot;
    private Vector3 bestCoverSpot;



    //private List<Transform> activeCovers = new List<Transform>(); 
    private List<Cover> activeCovers = new List<Cover>();
    //[SerializeField] private GameObject muzzleflash;

    private void Start() {
        eventSystem = FindObjectOfType<CallbackSystem.EventSystem>();

        eventSystem.RegisterListener<CallbackSystem.ModuleSpawnEvent>(LoadModule);
        eventSystem.RegisterListener<CallbackSystem.ModuleDeSpawnEvent>(UnLoadModule);

        Instance ??= this;
    }

    public GameObject getBullet {
        get { return bullet; }
    }

    /*     public void SetBestCoverSpot(Transform bestCoverSpot) {
            this.bestCoverSpot = bestCoverSpot;
        } */

    public void SetBestCoverSpot(Vector3 bestCoverSpot) {
        this.bestCoverSpot = bestCoverSpot;
    }


    /*     public Transform GetBestCoverSpot() {

            return bestCoverSpot;
        } */

    public Vector3 GetBestCoverSpot() {
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

    private Dictionary<Vector2Int, HashSet<Vector3>> potentialCoverSpots = new Dictionary<Vector2Int, HashSet<Vector3>>();

    public void AddCoverSpot(Vector3 coverSpot) {
        Vector2Int modulePos = DynamicGraph.Instance.GetModulePosFromWorldPos(coverSpot);
        if (!potentialCoverSpots.ContainsKey(modulePos)) {
            potentialCoverSpots.Add(modulePos, new HashSet<Vector3>());
        }
        if (!potentialCoverSpots[modulePos].Contains(coverSpot)) {
            potentialCoverSpots[modulePos].Add(coverSpot);
        }
    }


    public HashSet<Vector3> GetNearbyCoverSpots(Vector2Int module) {
        if (potentialCoverSpots.ContainsKey(module)) return potentialCoverSpots[module];
        return null;
    }



}
