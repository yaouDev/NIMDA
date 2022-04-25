using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public abstract class EventInfo
    {
        public string EventDescription;
    }

    public class UnitRespawnEI : EventInfo
    {
        public bool isGOPlayerOne;
    }

    public class UnitHealthUpdate : EventInfo
    {
        public bool isGOPlayerOne;
        public float health;
    }
}