using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour {
	[SerializeField]
	private Image[] batteryUIs;
	private int currentBattery = 5;
	
	[SerializeField] [Range(.0f, .1f)] 
	private float percentagePerSecondIncrease;
	
	[SerializeField] [Range(0.1f, 1.0f)]
	private float damageAmount;
	
	[SerializeField]
	private float[] batteryCharge = { 1.0f, 1.0f, 1.0f, 1.0f, .5f };

	[SerializeField] [Range(1, 6)]
	private int respawnBatteryAmount = 4;
	
	public bool takeDMG;
	public bool respawn;
	public bool alive = true;

	[SerializeField] private Transform player;
	[SerializeField] private Transform otherPlayer;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private float respawnTime = 12.0f;
	private float respawnTimer;

	private void Update() {
		
		if (alive) {
			respawnTimer = 0.0f;
			RechargeBattery();
		} else
			respawnTimer += Time.deltaTime;
		
		if (takeDMG) {
			takeDMG = false;
			TakeDamage();
		}

		if (!alive && respawnTimer > respawnTime) {
			respawn = false;
			Respawn();
		}
	}

	public void Respawn() {
		if (alive) 
			return;
		
		playerController.Respawn();
		
		alive = true;
		
		for (int i = 0; i < respawnBatteryAmount; i++) {
			batteryCharge[i] = 1.0f;
			batteryUIs[i].fillAmount = 1.0f;
			batteryUIs[i].gameObject.SetActive(true);
		}

		currentBattery = respawnBatteryAmount - 1;

		if (otherPlayer != null)
			player.transform.position = otherPlayer.transform.position + Vector3.one;
	}
	
	private void RechargeBattery() {

		batteryCharge[currentBattery] += Time.deltaTime * percentagePerSecondIncrease;
		// don't fill more than 100%
		batteryCharge[currentBattery] = Mathf.Min(batteryCharge[currentBattery], 1.0f);
		batteryUIs[currentBattery].fillAmount = batteryCharge[currentBattery];
	}

	public void AddBattery() {
		
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
	
	public void TakeDamage() {
		
		if (!alive) 
			return;
		
		batteryCharge[currentBattery] -= damageAmount;
		
		// don't empty last battery completely
		// if (currentBattery == 0) {
		// 	batteryCharge[currentBattery] = Mathf.Max(0.0f,  batteryCharge[currentBattery]);
		// }
		
		// if battery empty (+ small margin) and isn't last battery — remove current battery 
		if (batteryCharge[currentBattery] < -0.05f) {
			batteryUIs[currentBattery].gameObject.SetActive(false);
			currentBattery--;
		}

		if (currentBattery < 0) {
			Die(); // !!!!
		}
	}

	private void Die() {
		
		Debug.Log("You Died"); 
		alive = false;
		playerController.Die();
	}
}
