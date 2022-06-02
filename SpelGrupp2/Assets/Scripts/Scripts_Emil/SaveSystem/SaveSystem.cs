using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CallbackSystem;

public class SaveSystem : MonoBehaviour {

    private string path;

    public static SaveSystem Instance;
    private PlayerController pOneController, pTwoController;
    private PlayerAttack pOneAttack, pTwoAttack;
    private PlayerHealth pOneHealth, pTwoHealth;
    private Crafting pOneCrafting, pTwoCrafting;
    [SerializeField] private bool loadGame = true;
    private TabGroup tabGroup;

    private void Start() {
        Instance ??= this;
        tabGroup = FindObjectOfType<TabGroup>();
        path = Application.persistentDataPath + "/save.bin";
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject playerOne, playerTwo;
        PlayerAttack unknownPlayerAttack = players[0].GetComponent<PlayerAttack>();
        if (unknownPlayerAttack.IsPlayerOne()) {
            playerOne = players[0];
            playerTwo = players[1];
            pOneAttack = unknownPlayerAttack;
        } else {
            playerOne = players[1];
            playerTwo = players[0];
            pTwoAttack = unknownPlayerAttack;
        }

        if (pOneAttack == null) pOneAttack = playerOne.GetComponent<PlayerAttack>();
        pOneHealth = playerOne.GetComponent<PlayerHealth>();
        pOneCrafting = playerOne.GetComponent<Crafting>();
        pOneController = playerOne.GetComponent<PlayerController>();

        if (pTwoAttack == null) pTwoAttack = playerTwo.GetComponent<PlayerAttack>();
        pTwoHealth = playerTwo.GetComponent<PlayerHealth>();
        pTwoCrafting = playerTwo.GetComponent<Crafting>();
        pTwoController = playerTwo.GetComponent<PlayerController>();

        LoadGame();
    }

    public PlayerHealth PlayerOneHealth {
        get { return pOneHealth; }
    }

    public PlayerAttack PlayerOneAttack {
        get { return pOneAttack; }
    }

    public Crafting PlayerOneCrafting {
        get { return pOneCrafting; }
    }

    public PlayerController PlayerOneController {
        get { return pOneController; }
    }

    public PlayerHealth PlayerTwoHealth {
        get { return pTwoHealth; }
    }

    public PlayerAttack PlayerTwoAttack {
        get { return pTwoAttack; }
    }

    public Crafting PlayerTwoCrafting {
        get { return pTwoCrafting; }
    }

    public PlayerController PlayerTwoController {
        get { return pTwoController; }
    }

    public TabGroup TabGroup {
        get { return tabGroup; }
    }

    public void SaveGameData(bool enteringSafeRoom) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(enteringSafeRoom);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    private SaveData LoadGameData() {
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;

            stream.Close();
            return data;

        } else {
            // Debug.LogError("Save file not found");
            return null;
        }
    }

    [ContextMenu("Clear Save file")]
    public void ClearSaveFile() {
        if (File.Exists(Application.persistentDataPath + "/save.bin")) {
            File.Delete(Application.persistentDataPath + "/save.bin");
        }
    }

    public bool SaveExists {
        get { return File.Exists(Application.persistentDataPath + "/save.bin"); }
    }

    public void LoadGame() {
        SaveData data = LoadGameData();
        if (data != null && loadGame) {

            tabGroup.buttonsDictionary = data.buttonDict;
            tabGroup.InstantiateButtons();

            // playerOne
            pOneAttack.LaserDamageUpgraded = data.pOneLaserDmgUpgraded;
            pOneAttack.LaserBeamWidthUpgraded = data.pOneLaserBeamUpgraded;
            pOneAttack.LaserChargeRateUpgraded = data.pOneLaserChargeUpgraded;
            pOneAttack.ProjectileWeaponUpgraded = data.pOneProjectileUpgraded;
            pOneAttack.RevolverCritUpgraded = data.pOneRevolverCritUpgraded;
            pOneAttack.RevolverMagazineUpgraded = data.pOneRevolverMagUpgraded;
            pOneAttack.SetBulletsOnLoad(data.pOneAmmo);
            pOneHealth.SetBatteriesOnLoad(data.pOneBattery);
            pOneHealth.SetHealthOnLoad(data.pOneHealthPoints);
            pOneHealth.DecreaseDamageUpgraded = data.pOneDecreaseDmgUpgraded;
            pOneHealth.ChooseMaterialColor(new Color(data.pOneColor[0], data.pOneColor[1], data.pOneColor[2], data.pOneColor[3]));
            pOneCrafting.iron = data.pOneIron;
            pOneCrafting.copper = data.pOneCopper;
            pOneCrafting.transistor = data.pOneTransistor;
            pOneCrafting.currency = data.pOneCurrency;
            pOneAttack.transform.position = new Vector3(data.saferoomPosition[0], data.saferoomPosition[1], data.saferoomPosition[2]);
            pOneController.MovementSpeedUpgraded = data.pOneMovementSpeedUpgraded;


            // playerTwo
            pTwoAttack.LaserDamageUpgraded = data.pTwoLaserDmgUpgraded;
            pTwoAttack.LaserBeamWidthUpgraded = data.pTwoLaserBeamUpgraded;
            pTwoAttack.LaserChargeRateUpgraded = data.pTwoLaserChargeUpgraded;
            pTwoAttack.ProjectileWeaponUpgraded = data.pTwoProjectileUpgraded;
            pTwoAttack.RevolverCritUpgraded = data.pTwoRevolverCritUpgraded;
            pTwoAttack.RevolverMagazineUpgraded = data.pTwoRevolverMagUpgraded;
            pTwoAttack.SetBulletsOnLoad(data.pTwoAmmo);
            pTwoHealth.SetBatteriesOnLoad(data.pTwoBattery);
            pTwoHealth.SetHealthOnLoad(data.pTwoHealthPoints);
            pTwoHealth.DecreaseDamageUpgraded = data.pTwoDecreaseDmgUpgraded;
            pTwoHealth.ChooseMaterialColor(new Color(data.pTwoColor[0], data.pTwoColor[1], data.pTwoColor[2], data.pTwoColor[3]));
            pTwoCrafting.iron = data.pTwoIron;
            pTwoCrafting.copper = data.pTwoCopper;
            pTwoCrafting.transistor = data.pTwoTransistor;
            pTwoCrafting.currency = data.pTwoCurrency;
            pTwoAttack.transform.position = new Vector3(data.saferoomPosition[0], data.saferoomPosition[1], data.saferoomPosition[2]);
            pTwoController.MovementSpeedUpgraded = data.pTwoMovementSpeedUpgraded;
        }
    }

    public void LoadGameRestart() {
        tabGroup.buttonsDictionary = new System.Collections.Generic.Dictionary<string, bool>();
        tabGroup.InstantiateButtons();

        Vector3 startPos = new Vector3(-10.6f, 2.5f, -18.11f);

        // playerOne
        pOneAttack.LaserDamageUpgraded = false;
        pOneAttack.LaserBeamWidthUpgraded = false;
        pOneAttack.LaserChargeRateUpgraded = false;
        pOneAttack.ProjectileWeaponUpgraded = false;
        pOneAttack.RevolverCritUpgraded = false;
        pOneAttack.RevolverMagazineUpgraded = false;
        pOneAttack.SetBulletsOnLoad(5);
        pOneHealth.SetBatteriesOnLoad(3);
        pOneHealth.SetHealthOnLoad(100);
        pOneHealth.DecreaseDamageUpgraded = false;
        pOneHealth.ChooseMaterialColor();
        pOneCrafting.iron = 0;
        pOneCrafting.copper = 0;
        pOneCrafting.transistor = 0;
        pOneCrafting.currency = 0;
        pOneAttack.transform.position = startPos;
        pOneController.MovementSpeedUpgraded = false;


        // playerTwo
        pTwoAttack.LaserDamageUpgraded = false;
        pTwoAttack.LaserBeamWidthUpgraded = false;
        pTwoAttack.LaserChargeRateUpgraded = false;
        pTwoAttack.ProjectileWeaponUpgraded = false;
        pTwoAttack.RevolverCritUpgraded = false;
        pTwoAttack.RevolverMagazineUpgraded = false;
        pTwoAttack.SetBulletsOnLoad(5);
        pTwoHealth.SetBatteriesOnLoad(3);
        pTwoHealth.SetHealthOnLoad(100);
        pTwoHealth.DecreaseDamageUpgraded = false;
        pTwoHealth.ChooseMaterialColor();
        pTwoCrafting.iron = 0;
        pTwoCrafting.copper = 0;
        pTwoCrafting.transistor = 0;
        pTwoCrafting.currency = 0;
        pTwoAttack.transform.position = startPos;
        pOneAttack.transform.position = startPos;
        pTwoController.MovementSpeedUpgraded = false;
    }

}
