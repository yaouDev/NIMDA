using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    //[SerializeField] private List<Transform> coverSpots = new List<Transform>();
    [SerializeField] private Transform[] coverSpots; 
   /* public List<Transform> GetCoverSpots()
    {
        return coverSpots;
    }*/
   public Transform [] GetCoverSpots()
    {
        return coverSpots;
    }

}
