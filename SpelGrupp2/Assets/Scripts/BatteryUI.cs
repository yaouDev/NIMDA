using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine.UI;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private Image batteryUI, batteryIcon;
    [SerializeField] private TextMeshProUGUI batteryAmount;
    
    public void UpdateBatteryUI(float battery)
    {

        if (battery >= float.Epsilon)
        {
            batteryAmount.gameObject.SetActive(true);
            batteryUI.gameObject.SetActive(true);
            batteryUI.fillAmount = Ease.EaseInCirc(battery);
            batteryUI.color = Color.Lerp(Color.red, Color.green, Ease.EaseInCirc(battery));
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
}
