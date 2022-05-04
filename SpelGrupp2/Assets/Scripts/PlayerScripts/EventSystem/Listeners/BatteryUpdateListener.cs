using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class BatteryUpdateListener : MonoBehaviour
    {
        [SerializeField] public BatteryUI[] players;

        public void Start()
        {
            EventSystem.Current.RegisterListener<HealthUpdateEvent>(UpdateBattery);
        }

        private void UpdateBattery(HealthUpdateEvent uhu)
        {   
            BatteryUI battery = uhu.isPlayerOne ? players[0] : players[1];
            battery.UpdateBatteryUI(uhu.health);
        }
    }
}
