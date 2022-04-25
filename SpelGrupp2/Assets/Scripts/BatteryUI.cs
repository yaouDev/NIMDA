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
    [SerializeField]
    private Image[] batteryUIs;

    [SerializeField] [Range(.0f, .1f)] private float percentagePerSecondIncrease;
    //[SerializeField] [Range(0.1f, 1.0f)] private float damageAmount;

    [SerializeField] [Range(1, 6)] private int respawnBatteryAmount = 4;
    [SerializeField] private Transform player;
    [SerializeField] private Transform otherPlayer;
    [SerializeField] private Image laserMeter;

    //[SerializeField] private float[] batteryCharge = { 1.0f, 1.0f, 1.0f, 1.0f, .5f };
    public float battery = 1f;
    public bool alive = true;

    private void Update()
    {
        UpdateBatteryUI();
    }

    public void Respawn()
    {
        for (int i = 0; i < respawnBatteryAmount; i++)
        {
            batteryUIs[i].gameObject.SetActive(true);
        }
        if (otherPlayer != null)
            player.transform.position = otherPlayer.transform.position + Vector3.one;
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
	*/

    public void LaserBatteryDrain(float newHealth)
    {
        battery = newHealth;

        if (battery < 0.0f)
        {
        //    UpdateBatteryPercentage(Mathf.Abs(battery));
        }
    }

    private void UpdateBatteryUI()
    {
        laserMeter.fillAmount = Ease.EaseInCirc(battery);
        laserMeter.color = Color.Lerp(Color.yellow, Color.red, Ease.EaseInCirc(battery));
    }

    //public void UpdateBatteryPercentage() => UpdateBatteryPercentage(damageAmount);
    /*
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
    private void Die()
    {

        Debug.Log("You Died");
        alive = false;
    }
}
