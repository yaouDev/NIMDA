using System;
using System.Collections;
using System.Collections.Generic;
// using System.ComponentModel.Design.Serialization;
using UnityEngine;
// using UnityEditor.UI;
//using UnityEditorInternal;
using UnityEngine.UI;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private Image batteryUI;
    [SerializeField] private TextMeshProUGUI batteryAmount;
    [SerializeField] private GameObject prefab;
    
    public void UpdateBatteryUI(float battery)
    {

        if (battery >= float.Epsilon)
        {
            battery /= 100;
            batteryAmount.gameObject.SetActive(true);
            batteryUI.gameObject.SetActive(true);
            batteryUI.fillAmount = battery;
            batteryUI.color = Color.Lerp(Color.red, Color.green, battery);
        }
        else
        {
            batteryUI.fillAmount = 0f;
            batteryUI.gameObject.SetActive(false);
        }
    }

    public void SetBatteryCount(int count)
    {
        batteryAmount.text = count.ToString();
    }

    public void BreakBattery()
    {
        Instantiate(prefab, transform.position, transform.rotation, transform.parent);
    }
}
