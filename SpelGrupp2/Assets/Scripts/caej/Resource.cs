using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/Resource")]
public class Resource : ScriptableObject
{
    public new string name;
    public GameObject item;
    //h�r kan man l�gga in rarity, value, osv

}
