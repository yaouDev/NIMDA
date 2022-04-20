using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool inSafeZone = false;

    public bool alive = true;
    private float healthReg, superReg, standardReg; // TODO Safe zone regeneration buff 

    [SerializeField] private BatteryUI health;
    [SerializeField] private GameObject visuals;

    private void Update()
    {
        UpdateSafeZoneBuff();

    }

    public void TakeDamage()
    {
        Debug.Log("Took damage");
        health.TakeDamage();
    }

    public void Die()
    {
        alive = false;
        visuals.SetActive(false);
    }

    public void Respawn()
    {
        alive = true;
        visuals.SetActive(true);
    }

    public void DrainBattery()
    {
        health.LaserBatteryDrain();
    }

    public void UpdateSafeZoneBuff() // TODO Safe zone regeneration buff 
    {
        healthReg = inSafeZone ? superReg : standardReg;
    }
}
