using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CallbackSystem;

public class SaveSystem : MonoBehaviour {

    private string path;

    public static SaveSystem Instance;
    private PlayerAttack playerOneAttack, playerTwoAttack;
    private PlayerHealth playerOneHealth, playerTwoHealth;
    private Crafting playerOneCrafting, playerTwoCrafting;
    [SerializeField] private bool loadGame = true;

    private void Start() {
        Instance ??= this;
        path = Application.persistentDataPath + "/save.bin";
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject playerOne, playerTwo;
        PlayerAttack unknownPlayerAttack = players[0].GetComponent<PlayerAttack>();
        if (unknownPlayerAttack.IsPlayerOne()) {
            playerOne = players[0];
            playerTwo = players[1];
            playerOneAttack = unknownPlayerAttack;
        } else {
            playerOne = players[1];
            playerTwo = players[0];
            playerTwoAttack = unknownPlayerAttack;
        }

        if (playerOneAttack == null) playerOneAttack = playerOne.GetComponent<PlayerAttack>();
        playerOneHealth = playerOne.GetComponent<PlayerHealth>();
        playerOneCrafting = playerOne.GetComponent<Crafting>();

        if (playerTwoAttack == null) playerTwoAttack = playerTwo.GetComponent<PlayerAttack>();
        playerTwoHealth = playerTwo.GetComponent<PlayerHealth>();
        playerTwoCrafting = playerTwo.GetComponent<Crafting>();

        LoadGame();
    }

    public PlayerHealth PlayerOneHealth {
        get { return playerOneHealth; }
    }

    public PlayerAttack PlayerOneAttack {
        get { return playerOneAttack; }
    }

    public Crafting PlayerOneCrafting {
        get { return playerOneCrafting; }
    }

    public PlayerHealth PlayerTwoHealth {
        get { return playerTwoHealth; }
    }

    public PlayerAttack PlayerTwoAttack {
        get { return playerTwoAttack; }
    }

    public Crafting PlayerTwoCrafting {
        get { return playerTwoCrafting; }
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

    [ContextMenu("Clear savefile")]
    public void ClearSaveFile() {
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    private void LoadGame() {
        SaveData data = LoadGameData();
        if (data != null && loadGame) {
            // put all the info in the references here

            // playerOne
            playerOneAttack.LaserWeaponUpgraded = data.playerOneLaserUpgraded;
            playerOneAttack.ProjectileWeaponUpgraded = data.playerOneProjectileUpgraded;
            playerOneAttack.SetBulletsOnLoad(data.playerOneAmmo);
            playerOneHealth.SetBatteriesOnLoad(data.playerOneBattery);
            playerOneHealth.SetHealthOnLoad(data.playerOneHealth);
            playerOneHealth.ChooseMaterialColor(new Color(data.playerOneColor[0], data.playerOneColor[1], data.playerOneColor[2], data.playerOneColor[3]));
            playerOneCrafting.iron = data.playerOneIron;
            playerOneCrafting.copper = data.playerOneCopper;
            playerOneCrafting.transistor = data.playerOneTransistor;
            playerOneAttack.transform.position = new Vector3(data.saferoomPosition[0], data.saferoomPosition[1], data.saferoomPosition[2]);

            // playerTwo
            playerTwoAttack.LaserWeaponUpgraded = data.playerTwoLaserUpgraded;
            playerTwoAttack.ProjectileWeaponUpgraded = data.playerTwoProjectileUpgraded;
            playerTwoAttack.SetBulletsOnLoad(data.playerTwoAmmo);
            playerTwoHealth.SetBatteriesOnLoad(data.playerTwoBattery);
            playerTwoHealth.SetHealthOnLoad(data.playerTwoHealth);
            playerTwoHealth.ChooseMaterialColor(new Color(data.playerTwoColor[0], data.playerTwoColor[1], data.playerTwoColor[2], data.playerTwoColor[3]));
            playerTwoCrafting.iron = data.playerTwoIron;
            playerTwoCrafting.copper = data.playerTwoCopper;
            playerTwoCrafting.transistor = data.playerTwoTransistor;
            playerTwoAttack.transform.position = new Vector3(data.saferoomPosition[0], data.saferoomPosition[1], data.saferoomPosition[2]);
        }
    }

}
