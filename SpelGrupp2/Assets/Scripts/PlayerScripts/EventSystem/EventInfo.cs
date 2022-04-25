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
        public float healthPercentage;
    }

    public class UnitDeathEI : EventInfo
    {
        public bool isDead;
    }

    public class UnitRespawnEI : EventInfo
    {
        
    }

    public class UnitEnteredSafezone : EventInfo
    {
        public float batteryRegIncrease;
    }

    public class UnitHealthUpdate : EventInfo
    {
        public float health;
    }
}