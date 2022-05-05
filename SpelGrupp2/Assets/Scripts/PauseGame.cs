using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;
    private bool isPaused = false;
    
    void Start()
    {
        pauseScreen.SetActive(false);
    }
    
    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown("HELP"))
    //     {
    //         if (pauseScreen.activeInHierarchy)
    //         {
    //             Unpause();
    //         }
    //         if (!pauseScreen.activeInHierarchy)
    //         {
    //             Pause();
    //         }
    //     }
    // }

    public void PauseButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    private void Unpause()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
}
