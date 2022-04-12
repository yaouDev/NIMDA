using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/Resource")]
public class Resource : ScriptableObject
{
    public new string name;
    public GameObject item;
    //här kan man lägga in rarity, value, osv

}
