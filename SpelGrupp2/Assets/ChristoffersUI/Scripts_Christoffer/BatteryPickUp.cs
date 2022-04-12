using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPickUp : MonoBehaviour
{
    public BatteryUIs battery;
    
    

    private void Awake()
    {
          
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pang");
        if(battery.isOnlyOneBattery == true)
        {
            Debug.Log("1 batteri");
            battery.batteryBar.transform.position = new Vector2(140, 1340);
            battery.isOnlyOneBattery = false;
            battery.isOnlyTwoBatteries = true;
        }
        else if(battery.isOnlyTwoBatteries == true)
        {
            Debug.Log("2 batteri");
            battery.batteryBar.transform.position = new Vector2(200, 1340);
     /*     battery.script.enabled = false;
            battery.batteryBar3.SetActive(true);*/
            battery.isOnlyTwoBatteries = false;
            battery.isOnlyThreeBatteries = true; 

        } 
        else if(battery.isOnlyThreeBatteries == true)
        {
            battery.batteryBar.transform.position = new Vector2(260, 1340);
 /*           battery.script.enabled = false;
            battery.batteryBar4.SetActive(true);*/
            battery.isOnlyThreeBatteries = false;
            battery.isOnlyFourBatteries = true;
        }
        else if(battery.isOnlyFourBatteries == true)
        {
            battery.batteryBar.transform.position = new Vector2(320, 1340);
 /*           battery.script.enabled = false;
            battery.batteryBar5.SetActive(true);*/
            battery.isOnlyFourBatteries = false;
            battery.isOnlyFiveBatteries = true;
        }
        else if(battery.isOnlyFiveBatteries == true)
        {
            battery.batteryBar.transform.position = new Vector2(380, 1340);
  /*          battery.script.enabled = false;
            battery.batteryBar6.SetActive(true);*/
            battery.isOnlyFiveBatteries = false;
            battery.isSixBatteries = true;
        }

           /* for (int i = 0; i < battery.batteryBars.Length; i++)
            {
                if(battery.batteryBars.Length < 6) 
                {
                battery.batteryIndex += 1;
                Debug.Log("Mer batteri");
                }
                else
                {
                Debug.Log("Fullt Me batterier");
                }
            }*/
    }

}
