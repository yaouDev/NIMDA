using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public object Item;
    public int Amount;
}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemAmount> Materials;
    public List<ItemAmount> Results;

    public bool CanCraft(CallbackSystem.Crafting inventory)
    {

        return true;
    }

    public void Craft(CallbackSystem.Crafting inventory)
    {
        
    }
}
