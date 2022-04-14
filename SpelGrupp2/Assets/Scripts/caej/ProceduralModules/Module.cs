using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Modules/Module")]
public class Module : ScriptableObject
{
    public bool north;
    public bool south;
    public bool west;
    public bool east;

    public GameObject prefab;
}
