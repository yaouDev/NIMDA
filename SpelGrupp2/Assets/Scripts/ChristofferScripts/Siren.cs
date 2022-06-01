using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siren : MonoBehaviour
{
    private Light siren;
    void Start()
    {
        siren = GetComponentInChildren<Light>();
        siren.enabled = false; 
    }

}
