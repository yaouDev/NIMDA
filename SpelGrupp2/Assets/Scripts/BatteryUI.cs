using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private Image batteryUI;
    [SerializeField] private bool alive = true;

    public void UpdateBatteryUI(float battery)
    {
        if (battery > 0f)
        {
           // Debug.Log("Battery value: " + battery);
            batteryUI.fillAmount = Ease.EaseInCirc(battery);
            batteryUI.color = Color.Lerp(Color.red, Color.green, Ease.EaseInCirc(battery));
        }
        else
            Die();
    }

    public void Respawn()
    {
        alive = true;
        batteryUI.gameObject.SetActive(true);
    }

    public void Die()
    {
        batteryUI.gameObject.SetActive(false);
        alive = false;
    }

    /*
	private void RechargeBattery() {

		batteryCharge[currentBattery] += Time.deltaTime * percentagePerSecondIncrease;
		// don't fill more than 100%
		batteryCharge[currentBattery] = Mathf.Min(batteryCharge[currentBattery], 1.0f);
		batteryUIs[currentBattery].fillAmount = batteryCharge[currentBattery];
	}
	*/

    /*public void AddBattery() {
		
		if (currentBattery < 5) {
			// move recharging battery-fill up one step 
			batteryCharge[currentBattery + 1] = batteryCharge[currentBattery];
			batteryCharge[currentBattery] = 1.0f;
			
			// move recharging battery visually up one step
			batteryUIs[currentBattery + 1].gameObject.SetActive(true);
			batteryUIs[currentBattery].fillAmount = batteryCharge[currentBattery];
			
			currentBattery++;
		}
		else {
			// no room for more batteries, refill last one instead
			batteryCharge[currentBattery] = 1.0f;
		}
	}
	

    public void LaserBatteryDrain(float newHealth)
    {
        battery = newHealth;

        if (battery < 0.0f)
        {
            //    UpdateBatteryPercentage(Mathf.Abs(battery));
        }
    }



    //public void UpdateBatteryPercentage() => UpdateBatteryPercentage(damageAmount);
    
    public void UpdateBatteryPercentage(float damageAmount)
    {

        if (!alive)
            return;

        battery -= damageAmount;

        // don't empty last battery completely
        // if (currentBattery == 0) {
        // 	batteryCharge[currentBattery] = Mathf.Max(0.0f,  batteryCharge[currentBattery]);
        // }

        // if battery empty (+ small margin) and isn't last battery — remove current battery 
        if (batteryCharge[currentBattery] < -0.05f)
        {
            batteryUIs[currentBattery].gameObject.SetActive(false);
            currentBattery--;
        }

        if (currentBattery < 0)
        {
            Die(); // TODO call UnitDeathEvent
        }
    }
    */
}
