using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour {

    [SerializeField] private Button restartFromCheckPoint, restart, quit;

    private void Start() {
        restartFromCheckPoint.onClick.AddListener(RestartGameFromSave);
        restart.onClick.AddListener(RestartGame);
        quit.onClick.AddListener(Application.Quit);
    }

    private void Update() {
        if (SaveSystem.Instance.SaveExists) restartFromCheckPoint.interactable = true;
        else restartFromCheckPoint.interactable = false;
    }

    private void RestartGameFromSave(){
        SaveSystem.Instance.LoadGame();
    }

    private void RestartGame(){
        if(SaveSystem.Instance.SaveExists) SaveSystem.Instance.ClearSaveFile();
        SaveSystem.Instance.LoadGameRestart();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    
}
