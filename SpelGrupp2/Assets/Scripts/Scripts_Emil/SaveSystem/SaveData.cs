using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

[System.Serializable]
public class SaveData {

    public float[] saferoomPosition;

    // PlayerOne data
    public int playerOneIron;
    public int playerOneTransistor;
    public int playerOneCopper;
    public int playerOneBattery;
    public int playerOneAmmo;
    public float playerOneHealth;
    public float[] playerOneColor;
    public bool playerOneLaserUpgraded;
    public bool playerOneProjectileUpgraded;

    // PlayerTwo data
    public int playerTwoIron;
    public int playerTwoTransistor;
    public int playerTwoCopper;
    public int playerTwoBattery;
    public int playerTwoAmmo;
    public float playerTwoHealth;
    public float[] playerTwoColor;
    public bool playerTwoLaserUpgraded;
    public bool playerTwoProjectileUpgraded;


    public SaveData(bool enteringSafeRoom) {
        Vector3 pos;

        if (enteringSafeRoom) {
            pos = SaveSystem.Instance.PlayerOneAttack.transform.position.z > SaveSystem.Instance.PlayerTwoAttack.transform.position.z
            ? SaveSystem.Instance.PlayerOneAttack.transform.position : SaveSystem.Instance.PlayerTwoAttack.transform.position;
            pos.z += 4f;
        } else {
            pos = SaveSystem.Instance.PlayerOneAttack.transform.position.z < SaveSystem.Instance.PlayerTwoAttack.transform.position.z
            ? SaveSystem.Instance.PlayerOneAttack.transform.position : SaveSystem.Instance.PlayerTwoAttack.transform.position;
            pos.z -= 4f;
        }

        Vector2Int safeRoomModulePos = DynamicGraph.Instance.GetModulePosFromWorldPos(pos);
        saferoomPosition = new float[3];
        saferoomPosition[0] = safeRoomModulePos.x * 50;
        saferoomPosition[1] = pos.y;
        saferoomPosition[2] = safeRoomModulePos.y * 50;

        playerOneIron = SaveSystem.Instance.PlayerOneCrafting.iron;
        playerOneTransistor = SaveSystem.Instance.PlayerOneCrafting.transistor;
        playerOneCopper = SaveSystem.Instance.PlayerOneCrafting.copper;
        playerOneBattery = SaveSystem.Instance.PlayerOneHealth.GetCurrentBatteryCount();
        playerOneAmmo = SaveSystem.Instance.PlayerOneAttack.ReturnBullets();
        playerOneHealth = SaveSystem.Instance.PlayerOneHealth.GetCurrenthealth();
        playerOneLaserUpgraded = SaveSystem.Instance.PlayerOneAttack.LaserWeaponUpgraded;
        playerOneProjectileUpgraded = SaveSystem.Instance.PlayerOneAttack.ProjectileWeaponUpgraded;

        playerOneColor = new float[4];
        Color playerOneColorRef = SaveSystem.Instance.PlayerOneHealth.GetCurrentMaterialColor();
        playerOneColor[0] = playerOneColorRef.r;
        playerOneColor[1] = playerOneColorRef.g;
        playerOneColor[2] = playerOneColorRef.b;
        playerOneColor[3] = playerOneColorRef.a;

        playerTwoIron = SaveSystem.Instance.PlayerTwoCrafting.iron;
        playerTwoTransistor = SaveSystem.Instance.PlayerTwoCrafting.transistor;
        playerTwoCopper = SaveSystem.Instance.PlayerTwoCrafting.copper;
        playerTwoBattery = SaveSystem.Instance.PlayerTwoHealth.GetCurrentBatteryCount();
        playerTwoAmmo = SaveSystem.Instance.PlayerTwoAttack.ReturnBullets();
        playerTwoHealth = SaveSystem.Instance.PlayerTwoHealth.GetCurrenthealth();
        playerTwoLaserUpgraded = SaveSystem.Instance.PlayerTwoAttack.LaserWeaponUpgraded;
        playerTwoProjectileUpgraded = SaveSystem.Instance.PlayerTwoAttack.ProjectileWeaponUpgraded;

        playerTwoColor = new float[4];
        Color playerTwoColorRef = SaveSystem.Instance.PlayerTwoHealth.GetCurrentMaterialColor();
        playerTwoColor[0] = playerTwoColorRef.r;
        playerTwoColor[1] = playerTwoColorRef.g;
        playerTwoColor[2] = playerTwoColorRef.b;
        playerTwoColor[3] = playerTwoColorRef.a;
    }

}
