using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class HealthBarUpdateListener : MonoBehaviour
    {
        [Header("Assign both players BatteryUI component")]
        [SerializeField] private BatteryUI[] players;

        public void Start()
        {
            EventSystem.Current.RegisterListener<HealthUpdateEvent>(UpdateHealthBar);
        }

        private void UpdateHealthBar(HealthUpdateEvent uhu)
        {   
            BatteryUI battery = uhu.isPlayerOne ? players[0] : players[1];
            battery.SetBatteryCount(uhu.batteries);
            battery.UpdateBatteryUI(uhu.health);
            if (uhu.batteryDecreased)
                battery.BreakBattery();
        }
    }
}
