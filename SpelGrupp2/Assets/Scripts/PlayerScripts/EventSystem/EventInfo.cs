using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public abstract class EventInfo
    {
        public string EventDescription;
    }

    public class DecreaseBatteryEI : EventInfo
    {
        public bool isPlayerOne;
        public PlayerHealth healthInfo;
    }
}