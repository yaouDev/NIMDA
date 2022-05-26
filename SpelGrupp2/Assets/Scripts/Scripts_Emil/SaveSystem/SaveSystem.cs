using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CallbackSystem;

public class SaveSystem : MonoBehaviour {

    private string path = Application.persistentDataPath + "/save.bin";

    public static SaveSystem Instance;
    private PlayerAttack playerOneAttack, playerTwoAttack;
    private PlayerHealth playerOneHealth, playerTwoHealth;
    private Crafting playerOneCrafting, playerTwoCrafting;

    private void Awake() {
        Instance ??= this;
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
            Debug.LogError("Save file not found");
            return null;
        }
    }

    private void LoadGame(){
        SaveData data = LoadGameData();
        if(data != null){
            // put all the info in the references here
        }
    }

}
