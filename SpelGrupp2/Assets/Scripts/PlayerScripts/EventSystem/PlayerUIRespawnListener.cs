using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerUIRespawnListener : MonoBehaviour
    {
        [SerializeField] private BatteryUI[] playerUIs;

        void Start()
        {
            EventSystem.Current.RegisterListener<UnitRespawnEI>(RespawnBattery);
        }

        void RespawnBattery(UnitRespawnEI urei)
        {
            BatteryUI battery = urei.isGOPlayerOne ? playerUIs[0] : playerUIs[1];
            battery.Respawn();
        }
    }
}

