using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PointCounter : MonoBehaviour
{
    public static PointCounter instance;
    [SerializeField] TextMeshProUGUI pointCounterTmp;
    [HideInInspector] public int pointCount; 

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else 
        {
            //så det inte blir två i en scen
            Destroy(gameObject);
        }
    }
    
    public void UpdatePointCounterUI()
    {
        pointCounterTmp.text = pointCount.ToString();
    }

}
