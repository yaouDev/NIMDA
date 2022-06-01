using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallbackSystem
{
    public class CraftBullet : CraftableObject
    {
        override public void PerformAction(Crafting crafting)
        {
           // crafting.playerAttackScript.UpdateBulletCount(3);
        }
    }
}
