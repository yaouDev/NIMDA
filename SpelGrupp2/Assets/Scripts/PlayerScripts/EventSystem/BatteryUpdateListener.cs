using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class BatteryUpdateListener
    {
        [SerializeField] private BatteryUI[] playerUIs;

        void Start()
        {
            EventSystem.Current.RegisterListener<DecreaseBatteryEI>(DecreaseBattery);
        }

        void DecreaseBattery(DecreaseBatteryEI buei)
        {   
            BatteryUI battery = buei.isPlayerOne ? playerUIs[0] : playerUIs[1];
            battery.TakeDamage();
            buei.healthInfo.TakeDamage();
        }
    }
}
