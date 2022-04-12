using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            BatteryUI.instance.UseBattery(10);
        }

    }
}
