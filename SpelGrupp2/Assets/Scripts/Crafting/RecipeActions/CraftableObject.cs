using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public abstract class CraftableObject : MonoBehaviour
    {
        public abstract void PerformAction(Crafting crafting);
    }
}

