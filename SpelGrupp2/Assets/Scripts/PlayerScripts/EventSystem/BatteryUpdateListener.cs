using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class BatteryUpdateListener : MonoBehaviour
    {
        [SerializeField] private BatteryUI[] playerUIs;

        public void Start()
        {
            EventSystem.Current.RegisterListener<UnitHealthUpdate>(UpdateBattery);
        }

        private void UpdateBattery(UnitHealthUpdate uhu)
        {   
            BatteryUI battery = uhu.isGOPlayerOne ? playerUIs[0] : playerUIs[1];
            battery.UpdateBatteryUI(uhu.health);
        }
    }
}
