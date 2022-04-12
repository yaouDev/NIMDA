using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour {

	private Image[] batteryUI;
	private int currentBattery = 6;
	private float[] batteryCharge = { 100, 100, 100, 100, 100, 100 };
	
	private void Update() {
		RechargeBattery();	
	}

	private void RechargeBattery() {
		
	}
}
