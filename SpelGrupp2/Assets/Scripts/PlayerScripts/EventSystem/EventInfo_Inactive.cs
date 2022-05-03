using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public abstract class EventInfo_Inactive
    {
        public string EventDescription;
        public bool isGOPlayerOne;
        public float health;
    }

    public class UnitHealthUpdate : EventInfo_Inactive
    {

    }

    public class UnitRespawnEI : EventInfo_Inactive
    {

    }
}