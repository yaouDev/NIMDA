using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/Resource")]
public class Resource : ScriptableObject
{
    public new string name;
    public GameObject item;
    //here kan man laegga in rarity, value, osv

}
