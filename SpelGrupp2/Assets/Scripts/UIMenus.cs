using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenus : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject gameOverScreen;
    private bool isPaused = false;
    private int deadPlayerCount;

    void Start()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void PauseButton(InputAction.CallbackContext context)
    {
        if (context.performed && deadPlayerCount != 2)
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

    private void GameOver()
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
    }

    public void DeadPlayers(int number)
    {
        deadPlayerCount += number;
        if (deadPlayerCount == 2)
        {
            GameOver();
        }
    }
}
