using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public class PlayerUIDeathListener : MonoBehaviour
    {
        [SerializeField] public BatteryUI[] players;
        //private BatteryUI[] players;

        void Start()
        {
            //players = GetComponentInParent<UIHolder>().playerUIs;
            EventSystem.Current.RegisterListener<UnitDeathEI>(KillBattery);
        }

        void KillBattery(UnitDeathEI urei)
        {
            BatteryUI battery = urei.isGOPlayerOne ? players[0] : players[1];
            Debug.Log("Battery name: " + battery);
            battery.Die();
        }
    }
}

