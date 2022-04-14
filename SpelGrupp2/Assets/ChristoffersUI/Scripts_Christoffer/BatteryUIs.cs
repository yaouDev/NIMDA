using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryUIs : MonoBehaviour
{
    public Slider batteryBar;
    public BatteryUI script;
    public GameObject batteryPicture;
    public Transform [] picturePositions;

    public int picturePositionIndex;
    public bool isOnlyOneBattery = true;
    public bool isOnlyTwoBatteries = false;
    public bool isOnlyThreeBatteries = false;
    public bool isOnlyFourBatteries = false;
    public bool isOnlyFiveBatteries = false;
    public bool isSixBatteries = false;

    private int maxBattery = 100;
    private int currentBattery;


    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen; 

    public static BatteryUIs instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        isOnlyOneBattery = true;
        FillBattery();
    }

    public void UseBattery(int amount)
    {
        if(currentBattery - amount >= 0)
        {
            currentBattery -= amount;
            batteryBar.value = currentBattery;

            if(regen != null)
            {
                StopCoroutine(regen);
            }

            regen = StartCoroutine(RegenBattery());
        }
        else
        {
            SwitchToNextBattery();
            Debug.Log("Not eniugh battery");
        }
    }
    private IEnumerator RegenBattery()
    {
        yield return new WaitForSeconds(2);

        while(currentBattery < maxBattery)
        {
            currentBattery += maxBattery / 100;
            batteryBar.value = currentBattery;
            yield return regenTick;
        }
        regen = null; 
    }


    private void SwitchToNextBattery()
    {
        if (isSixBatteries == true)
        {
            batteryBar.transform.position = new Vector2(320, 1340);
            FillBattery();
            /*            batteryBar6.SetActive(false);*/
            isSixBatteries = false;
            isOnlyFiveBatteries = true; 
        }
        else if (isOnlyFiveBatteries == true) 
        {
            batteryBar.transform.position = new Vector2(260, 1340);
            FillBattery();
            //batteryBar5.SetActive(false);
            isOnlyFiveBatteries = false;
            isOnlyFourBatteries = true; 
        } 
        else if(isOnlyFourBatteries == true)
        {
            batteryBar.transform.position = new Vector2(200, 1340);
            FillBattery();
            //batteryBar4.SetActive(false);
            isOnlyFourBatteries = false;
            isOnlyThreeBatteries = true; 
        }
        else if (isOnlyThreeBatteries == true)
        {
            batteryBar.transform.position = new Vector2(140, 1340);
            FillBattery();
            //batteryBar3.SetActive(false);
            isOnlyThreeBatteries = false;
            isOnlyTwoBatteries = true; 
        }
        else if (isOnlyTwoBatteries == true)
        {
            batteryBar.transform.position = new Vector2(80, 1340);
            FillBattery();
            //batteryBar2.SetActive(false);
            isOnlyTwoBatteries = false;
            isOnlyOneBattery = true; 
        }
    }

    private void FillBattery()
    {
        currentBattery = maxBattery;
        batteryBar.maxValue = maxBattery;
        batteryBar.value = maxBattery;
    }

}
