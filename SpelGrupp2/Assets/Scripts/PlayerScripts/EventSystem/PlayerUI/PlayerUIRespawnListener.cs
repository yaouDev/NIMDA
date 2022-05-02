using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerUIRespawnListener : MonoBehaviour
    {
        [SerializeField] public BatteryUI[] players;
        //private BatteryUI[] players;

        void Start()
        {
            //players = GetComponentInParent<UIHolder>().playerUIs;
            EventSystem.Current.RegisterListener<UnitRespawnEI>(RespawnBattery);
        }

        void RespawnBattery(UnitRespawnEI urei)
        {
            BatteryUI battery = urei.isGOPlayerOne ? players[0] : players[1];
            battery.Respawn();
        }
    }
}
