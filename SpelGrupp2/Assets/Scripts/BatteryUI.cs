using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour {

	private Image[] batteryUIs;
	private int currentBattery = 5;
	[SerializeField] [Range(1.0f, 10.0f)] private float percentagePerSecondIncrease;
	private float[] batteryCharge = { 100, 100, 100, 100, 100, 100 };

	private void Update() {
		RechargeBattery();
	}

	private void RechargeBattery() {
		if (batteryCharge[currentBattery] < -5.0f && currentBattery > 0) {
			currentBattery--;
		}

		batteryCharge[currentBattery] += Time.deltaTime * percentagePerSecondIncrease;
	}
}
