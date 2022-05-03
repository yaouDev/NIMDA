using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class BatteryUpdateListener : MonoBehaviour
    {
        [SerializeField] public BatteryUI[] players;

        public void Start()
        {
            EventSystem.Current.RegisterListener<UnitHealthUpdate>(UpdateBattery);
        }

        private void UpdateBattery(UnitHealthUpdate uhu)
        {   
            BatteryUI battery = uhu.isGOPlayerOne ? players[0] : players[1];
            battery.UpdateBatteryUI(uhu.health);
        }
    }
}
