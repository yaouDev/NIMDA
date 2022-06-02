using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button resumeButton;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveSystem.Instance.SaveExists)
        {
            resumeButton.interactable = true;
        }
        else
        {
            resumeButton.interactable = false;
        }
    }

    public void ResumeGame()
    {
        SceneManager.LoadScene("Playtest");
    }

    public void NewGame()
    {
        SaveSystem.Instance.ClearSaveFile();
        SceneManager.LoadScene("Playtest");   
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
