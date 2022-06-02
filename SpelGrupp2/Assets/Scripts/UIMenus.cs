using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenus : MonoBehaviour
{

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject paused;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject crafting;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject blur;
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
        blur.SetActive(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        paused.SetActive(true);
        pauseScreen.SetActive(false);
        blur.SetActive(false);
    }

    public void ShowControls()
    {
        paused.SetActive(false);
        controls.SetActive(true);
    }

    public void ShowCrafting()
    {
        paused.SetActive(false);
        crafting.SetActive(true);
    }

    public void BackToPaused()
    {
        controls.SetActive(false);
        crafting.SetActive(false);
        paused.SetActive(true);
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

    public void GameWon()
    {
        Time.timeScale = 0;
        blur.SetActive(true);
        victoryScreen.SetActive(true);
    }
}
