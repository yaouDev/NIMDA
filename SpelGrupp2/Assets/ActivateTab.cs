using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateTab : MonoBehaviour
{
    [SerializeField] private Tab SwitchToTab;
    private TabGroup tabGroup;

    private enum Tab
    {
        Main, Laser, Revolver, Color
    }


    private void Start()
    {
        tabGroup = GetComponentInParent<TabGroup>();
    }
    public void ChooseTab()
    {
       // tabGroup.SwitchArray(sw)
    }
}
