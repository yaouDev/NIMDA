using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackSystem;

[System.Serializable]
public class SaveData {

    public float[] saferoomPosition;


    // PlayerOne data
    public int pOneIron;
    public int pOneTransistor;
    public int pOneCopper;
    public int pOneBattery;
    public int pOneAmmo;
    public int pOneCurrency;
    public float pOneHealthPoints;
    public float[] pOneColor;
    public bool pOneLaserDmgUpgraded;
    public bool pOneLaserChargeUpgraded;
    public bool pOneLaserBeamUpgraded;
    public bool pOneProjectileUpgraded;
    public bool pOneRevolverCritUpgraded;
    public bool pOneRevolverMagUpgraded;
    public bool pOneDecreaseDmgUpgraded;
    public bool pOneMovementSpeedUpgraded;
    public Dictionary<string, bool> pOneButtonDict;

    // PlayerTwo data
    public int pTwoIron;
    public int pTwoTransistor;
    public int pTwoCopper;
    public int pTwoBattery;
    public int pTwoAmmo;
    public int pTwoCurrency;
    public float pTwoHealthPoints;
    public float[] pTwoColor;
    public bool pTwoLaserDmgUpgraded;
    public bool pTwoLaserChargeUpgraded;
    public bool pTwoLaserBeamUpgraded;
    public bool pTwoProjectileUpgraded;
    public bool pTwoRevolverCritUpgraded;
    public bool pTwoRevolverMagUpgraded;
    public bool pTwoDecreaseDmgUpgraded;
    public bool pTwoMovementSpeedUpgraded;
    public Dictionary<string, bool> pTwoButtonDict;

    public SaveData(bool enteringSafeRoom) {
        Vector3 pos;

        pOneButtonDict = SaveSystem.Instance.PlayerOneTabGroup.buttonsDictionary;
        pTwoButtonDict = SaveSystem.Instance.PlayerTwoTabGroup.buttonsDictionary;

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

        pOneIron = SaveSystem.Instance.PlayerOneCrafting.iron;
        pOneTransistor = SaveSystem.Instance.PlayerOneCrafting.transistor;
        pOneCopper = SaveSystem.Instance.PlayerOneCrafting.copper;
        pOneBattery = SaveSystem.Instance.PlayerOneHealth.GetCurrentBatteryCount();
        pOneAmmo = SaveSystem.Instance.PlayerOneAttack.ReturnBullets();
        pOneCurrency = SaveSystem.Instance.PlayerOneCrafting.currency;
        pOneHealthPoints = SaveSystem.Instance.PlayerOneHealth.GetCurrenthealth();
        pOneLaserDmgUpgraded = SaveSystem.Instance.PlayerOneAttack.LaserDamageUpgraded;
        pOneLaserBeamUpgraded = SaveSystem.Instance.PlayerOneAttack.LaserBeamWidthUpgraded;
        pOneLaserChargeUpgraded = SaveSystem.Instance.PlayerOneAttack.LaserChargeRateUpgraded;
        pOneProjectileUpgraded = SaveSystem.Instance.PlayerOneAttack.ProjectileWeaponUpgraded;
        pOneRevolverCritUpgraded = SaveSystem.Instance.PlayerOneAttack.RevolverCritUpgraded;
        pOneRevolverMagUpgraded = SaveSystem.Instance.PlayerOneAttack.RevolverMagazineUpgraded;
        pOneDecreaseDmgUpgraded = SaveSystem.Instance.PlayerOneHealth.DecreaseDamageUpgraded;
        pOneMovementSpeedUpgraded = SaveSystem.Instance.PlayerOneController.MovementSpeedUpgraded;
        pOneColor = new float[4];
        Color pOneColorRef = SaveSystem.Instance.PlayerOneHealth.GetCurrentMaterialColor();
        pOneColor[0] = pOneColorRef.r;
        pOneColor[1] = pOneColorRef.g;
        pOneColor[2] = pOneColorRef.b;
        pOneColor[3] = pOneColorRef.a;

        pTwoIron = SaveSystem.Instance.PlayerTwoCrafting.iron;
        pTwoTransistor = SaveSystem.Instance.PlayerTwoCrafting.transistor;
        pTwoCopper = SaveSystem.Instance.PlayerTwoCrafting.copper;
        pTwoBattery = SaveSystem.Instance.PlayerTwoHealth.GetCurrentBatteryCount();
        pTwoAmmo = SaveSystem.Instance.PlayerTwoAttack.ReturnBullets();
        pTwoCurrency = SaveSystem.Instance.PlayerTwoCrafting.currency;
        pTwoHealthPoints = SaveSystem.Instance.PlayerTwoHealth.GetCurrenthealth();
        pTwoLaserDmgUpgraded = SaveSystem.Instance.PlayerTwoAttack.LaserDamageUpgraded;
        pTwoLaserBeamUpgraded = SaveSystem.Instance.PlayerTwoAttack.LaserBeamWidthUpgraded;
        pTwoLaserChargeUpgraded = SaveSystem.Instance.PlayerTwoAttack.LaserChargeRateUpgraded;
        pTwoProjectileUpgraded = SaveSystem.Instance.PlayerTwoAttack.ProjectileWeaponUpgraded;
        pTwoRevolverCritUpgraded = SaveSystem.Instance.PlayerTwoAttack.RevolverCritUpgraded;
        pTwoRevolverMagUpgraded = SaveSystem.Instance.PlayerTwoAttack.RevolverMagazineUpgraded;
        pTwoDecreaseDmgUpgraded = SaveSystem.Instance.PlayerTwoHealth.DecreaseDamageUpgraded;
        pTwoMovementSpeedUpgraded = SaveSystem.Instance.PlayerTwoController.MovementSpeedUpgraded;

        pTwoColor = new float[4];
        Color pTwoColorRef = SaveSystem.Instance.PlayerTwoHealth.GetCurrentMaterialColor();
        pTwoColor[0] = pTwoColorRef.r;
        pTwoColor[1] = pTwoColorRef.g;
        pTwoColor[2] = pTwoColorRef.b;
        pTwoColor[3] = pTwoColorRef.a;
    }

}
