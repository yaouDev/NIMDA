using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Callbacks
{
    public abstract class EventInfo
    {
        public string EventDescription;
        public bool isGOPlayerOne;
        public float health;
    }

    public class UnitHealthUpdate : EventInfo
    {

    }

    public class UnitRespawnEI : EventInfo
    {

    }
}